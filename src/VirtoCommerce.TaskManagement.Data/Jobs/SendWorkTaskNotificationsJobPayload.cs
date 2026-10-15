using System.Collections.Generic;
using VirtoCommerce.TaskManagement.Core.Models;

namespace VirtoCommerce.TaskManagement.Data.Jobs
{
    /// <summary>
    /// Payload of the background job that notifies the responsible persons of newly assigned work tasks.
    /// </summary>
    public class SendWorkTaskNotificationsJobPayload
    {
        /// <summary>The work tasks whose responsible persons have to be notified.</summary>
        public IList<WorkTask> WorkTasks { get; set; } = [];
    }
}
