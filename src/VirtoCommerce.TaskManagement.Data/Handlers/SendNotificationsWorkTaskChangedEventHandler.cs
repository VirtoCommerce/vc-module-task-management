using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hangfire;
using Microsoft.AspNetCore.Http;
using VirtoCommerce.CustomerModule.Core.Services;
using VirtoCommerce.NotificationsModule.Core.Extensions;
using VirtoCommerce.NotificationsModule.Core.Services;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Events;
using VirtoCommerce.Platform.Core.Settings;
using VirtoCommerce.TaskManagement.Core;
using VirtoCommerce.TaskManagement.Core.Events;
using VirtoCommerce.TaskManagement.Core.Models;
using VirtoCommerce.TaskManagement.Core.Notifications;

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
            INotificationSearchService notificationSearchService,
            IHttpContextAccessor httpContextAccessor)
        {
            _memberService = memberService;
            _notificationSender = notificationSender;
            _settingsManager = settingsManager;
            _notificationSearchService = notificationSearchService;
        }

        public virtual Task Handle(WorkTaskChangedEvent message)
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

            BackgroundJob.Enqueue(() => TryToSendOrderNotificationsAsync(tasks));

            return Task.CompletedTask;
        }

        public virtual async Task TryToSendOrderNotificationsAsync(IList<WorkTask> tasks)
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
                    notification.From = _settingsManager.GetValueByDescriptor<string>(ModuleConstants.Settings.General.TaskNotificationNoReplyEmail);

                    var baseUrl = _settingsManager.GetValueByDescriptor<string>(ModuleConstants.Settings.General.TaskAppTaskDetailsBaseUrl);
                    notification.WorkTaskUrl = $"{baseUrl}/{task.Id}";

                    await _notificationSender.ScheduleSendNotificationAsync(notification);
                }
            }
        }
    }
}
