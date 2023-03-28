using VirtoCommerce.Platform.Core.Security;

namespace VirtoCommerce.TaskManagement.Data.Authorization
{
    public sealed class TaskAssignToMeScope : PermissionScope
    {
        public TaskAssignToMeScope()
        {
            Label = "me";
            Scope = "me";
        }
    }
}
