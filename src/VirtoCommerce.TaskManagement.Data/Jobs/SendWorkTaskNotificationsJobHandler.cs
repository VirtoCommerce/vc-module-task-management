using System.Threading;
using System.Threading.Tasks;
using VirtoCommerce.Platform.Core.Jobs;
using VirtoCommerce.TaskManagement.Data.Handlers;

namespace VirtoCommerce.TaskManagement.Data.Jobs
{
    /// <summary>
    /// Sends the notifications about newly assigned work tasks, off the request thread.
    /// </summary>
    /// <remarks>
    /// Delegates to <see cref="SendNotificationsWorkTaskChangedEventHandler"/>, which still owns the logic and stays callable by
    /// background jobs enqueued by an earlier version that reference its method by name.
    /// </remarks>
    public class SendWorkTaskNotificationsJobHandler(SendNotificationsWorkTaskChangedEventHandler eventHandler) : IBackgroundJobHandler<SendWorkTaskNotificationsJobPayload>
    {
        public virtual Task Execute(SendWorkTaskNotificationsJobPayload payload, IJobExecutionContext context, CancellationToken cancellationToken = default)
        {
            return eventHandler.SendNotificationsAsync(payload.WorkTasks);
        }
    }
}
