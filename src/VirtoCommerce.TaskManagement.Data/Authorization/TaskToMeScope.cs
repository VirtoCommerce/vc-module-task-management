using VirtoCommerce.Platform.Core.Security;

namespace VirtoCommerce.TaskManagement.Data.Authorization
{
    public sealed class TaskToMeScope : PermissionScope
    {
        public TaskToMeScope()
        {
            Label = "me";
            Scope = "me";
        }
    }
}
