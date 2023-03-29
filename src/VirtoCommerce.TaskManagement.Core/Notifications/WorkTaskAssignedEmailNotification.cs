using VirtoCommerce.NotificationsModule.Core.Model;
using VirtoCommerce.TaskManagement.Core.Models;

namespace VirtoCommerce.TaskManagement.Core.Notifications
{
    public class WorkTaskAssignedEmailNotification : EmailNotification
    {
        public WorkTaskAssignedEmailNotification() : base(nameof(WorkTaskAssignedEmailNotification))
        {

        }

        public WorkTask WorkTask { get; set; }
        public string WorkTaskUrl { get; set; }
    }
}
