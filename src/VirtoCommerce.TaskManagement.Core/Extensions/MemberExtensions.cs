using System;
using System.Linq;
using VirtoCommerce.CustomerModule.Core.Model;

namespace VirtoCommerce.TaskManagement.Core.Extensions
{
    public static class MemberExtensions
    {
        [Obsolete("Use GetMemberDefaultOrganizationId")]
        public static string GetMemberOrganizationId(this Member member)
        {
            return member?.MemberType switch
            {
                nameof(Contact) => (member as Contact)?.Organizations?.FirstOrDefault(),
                nameof(Employee) => (member as Employee)?.Organizations?.FirstOrDefault(),
                _ => null
            };
        }

        public static string GetMemberDefaultOrganizationId(this Member member)
        {
            return member?.MemberType switch
            {
                nameof(Contact) => (member as Contact)?.DefaultOrganizationId,
                nameof(Employee) => (member as Employee)?.DefaultOrganizationId,
                _ => null
            };
        }
    }
}
