using VirtoCommerce.Platform.Security.Authorization;

namespace VirtoCommerce.TaskManagement.Data.Authorization
{
    public class TaskAuthorizationRequirement : PermissionAuthorizationRequirement
    {
        public TaskAuthorizationRequirement(string permission) : base(permission)
        {
        }
    }
}
