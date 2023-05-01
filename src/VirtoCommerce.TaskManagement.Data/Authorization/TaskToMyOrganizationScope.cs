using VirtoCommerce.Platform.Core.Security;

namespace VirtoCommerce.TaskManagement.Data.Authorization
{
    public sealed class TaskToMyOrganizationScope : PermissionScope
    {
        public TaskToMyOrganizationScope()
        {
            Label = "organization";
            Scope = "organization";
        }
    }
}
