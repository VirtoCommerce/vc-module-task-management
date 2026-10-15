using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.CustomerModule.Core.Services;
using VirtoCommerce.NotificationsModule.Core.Extensions;
using VirtoCommerce.NotificationsModule.Core.Services;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Events;
using VirtoCommerce.Platform.Core.Jobs;
using VirtoCommerce.Platform.Core.Settings;
using VirtoCommerce.TaskManagement.Core;
using VirtoCommerce.TaskManagement.Core.Events;
using VirtoCommerce.TaskManagement.Core.Models;
using VirtoCommerce.TaskManagement.Core.Notifications;
using VirtoCommerce.TaskManagement.Data.Jobs;

namespace VirtoCommerce.TaskManagement.Data.Handlers
{
    public class SendNotificationsWorkTaskChangedEventHandler : IEventHandler<WorkTaskChangedEvent>
    {
        private readonly IMemberService _memberService;
        private readonly INotificationSender _notificationSender;
        private readonly ISettingsManager _settingsManager;
        private readonly INotificationSearchService _notificationSearchService;

        public SendNotificationsWorkTaskChangedEventHandler(
            IMemberService memberService,
            INotificationSender notificationSender,
            ISettingsManager settingsManager,
            INotificationSearchService notificationSearchService)
        {
            _memberService = memberService;
            _notificationSender = notificationSender;
            _settingsManager = settingsManager;
            _notificationSearchService = notificationSearchService;
        }

        public virtual async Task Handle(WorkTaskChangedEvent message)
        {
            if (_settingsManager.GetValue<bool>(ModuleConstants.Settings.General.TaskNotificationsEnabled))
            {
                var tasks = message.ChangedEntries
                    .Where(changedEntry =>
                        (changedEntry.EntryState == EntryState.Added &&
                         !string.IsNullOrEmpty(changedEntry.NewEntry.ResponsibleId))
                        || (changedEntry.EntryState == EntryState.Modified
                            && changedEntry.OldEntry.ResponsibleId != changedEntry.NewEntry.ResponsibleId
                            && !string.IsNullOrEmpty(changedEntry.NewEntry.ResponsibleId)))
                    .Select(changedEntry => changedEntry.NewEntry)
                    .ToList();

                if (tasks.Count > 0)
                {
                    var payload = AbstractTypeFactory<SendWorkTaskNotificationsJobPayload>.TryCreateInstance();
                    payload.WorkTasks = tasks;

                    //The static facade, not an injected IBackgroundJob: RegisterEventHandler resolves this handler once
                    //from the root provider and holds it for the process lifetime, so it must not capture a Scoped dependency.
                    await BackgroundJob.Enqueue<SendWorkTaskNotificationsJobHandler>(payload);
                }
            }
        }

        /// <summary>
        /// Kept for background jobs enqueued by an earlier version, which reference this method by name.
        /// New work goes through <see cref="SendWorkTaskNotificationsJobHandler"/>; remove this once no such job
        /// can still be pending.
        /// </summary>
        [Obsolete("Enqueued indirectly by legacy Hangfire jobs only; new work uses SendWorkTaskNotificationsJobHandler.", DiagnosticId = "VC0015", UrlFormat = "https://docs.virtocommerce.org/products/products-virto3-versions")]
        public virtual Task TryToSendOrderNotificationsAsync(IList<WorkTask> tasks)
        {
            return SendNotificationsAsync(tasks);
        }

        public virtual async Task SendNotificationsAsync(IList<WorkTask> tasks)
        {
            var memberIds = tasks.Select(task => task.ResponsibleId).Distinct().ToArray();
            var contacts = await _memberService.GetByIdsAsync(memberIds, "WithEmails");
            foreach (var task in tasks)
            {
                var contact = contacts.FirstOrDefault(c => c.Id == task.ResponsibleId && c.Emails.Any());
                if (contact != null)
                {
                    var notification = await _notificationSearchService.GetNotificationAsync<WorkTaskAssignedEmailNotification>();
                    notification.WorkTask = task;
                    notification.To = contact.Emails.FirstOrDefault();
                    notification.From = _settingsManager.GetValue<string>(ModuleConstants.Settings.General.TaskNotificationNoReplyEmail);

                    var baseUrl = _settingsManager.GetValue<string>(ModuleConstants.Settings.General.TaskAppBaseUrl);
                    notification.WorkTaskUrl = $"{baseUrl}/#task/{task.Id}";

                    await _notificationSender.ScheduleSendNotificationAsync(notification);
                }
            }
        }
    }
}
