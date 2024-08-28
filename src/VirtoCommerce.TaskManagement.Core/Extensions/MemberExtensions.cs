using System.Linq;
using VirtoCommerce.CustomerModule.Core.Model;

namespace VirtoCommerce.TaskManagement.Core.Extensions
{
    public static class MemberExtensions
    {
        public static string GetMemberOrganizationId(this Member member)
        {
            return member switch
            {
                Contact contact => contact.DefaultOrganizationId ?? contact.Organizations?.FirstOrDefault(),
                Employee employee => employee.DefaultOrganizationId ?? employee.Organizations?.FirstOrDefault(),
                _ => null,
            };
        }
    }
}
