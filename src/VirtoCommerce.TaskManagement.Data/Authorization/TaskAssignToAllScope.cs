using VirtoCommerce.Platform.Core.Security;

namespace VirtoCommerce.TaskManagement.Data.Authorization
{
    public sealed class TaskAssignToAllScope : PermissionScope
    {
        public TaskAssignToAllScope()
        {
            Label = "all";
            Scope = "all";
        }
    }
}
