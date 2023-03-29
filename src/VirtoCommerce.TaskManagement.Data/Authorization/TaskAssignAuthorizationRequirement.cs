using VirtoCommerce.Platform.Security.Authorization;

namespace VirtoCommerce.TaskManagement.Data.Authorization
{
    public sealed class TaskAssignAuthorizationRequirement : PermissionAuthorizationRequirement
    {
        public TaskAssignAuthorizationRequirement(string permission)
            : base(permission)
        {
        }
    }
}
