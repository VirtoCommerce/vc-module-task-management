using VirtoCommerce.Platform.Core.Security;

namespace VirtoCommerce.TaskManagement.Data.Authorization
{
    public sealed class TaskAssignToMyOrganizationScope : PermissionScope
    {
        public TaskAssignToMyOrganizationScope()
        {
            Label = "organization";
            Scope = "organization";
        }
    }
}
