using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using VirtoCommerce.CustomerModule.Core.Model;
using VirtoCommerce.CustomerModule.Core.Model.Search;
using VirtoCommerce.CustomerModule.Core.Services;
using VirtoCommerce.Platform.Core.Security;
using VirtoCommerce.Platform.Security.Authorization;
using VirtoCommerce.TaskManagement.Core.Models;
using VirtoCommerce.TaskManagement.Data.Authorization;

namespace VirtoCommerce.TaskManagement.Web.Authorization
{
    public sealed class TaskAssignAuthorizationHandler : PermissionAuthorizationHandlerBase<TaskAssignAuthorizationRequirement>
    {
        public const string MemberIdClaimType = "memberId";

        private readonly MvcNewtonsoftJsonOptions _jsonOptions;
        private readonly IMemberService _memberService;
        private readonly IMemberSearchService _memberSearchService;

        public TaskAssignAuthorizationHandler(
            IOptions<MvcNewtonsoftJsonOptions> jsonOptions,
            IMemberService memberService,
            IMemberSearchService memberSearchService
            )
        {
            _jsonOptions = jsonOptions.Value;
            _memberService = memberService;
            _memberSearchService = memberSearchService;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, TaskAssignAuthorizationRequirement requirement)
        {
            await base.HandleRequirementAsync(context, requirement);

            if (!context.HasSucceeded)
            {
                var userPermission = context.User.FindPermission(requirement.Permission, _jsonOptions.SerializerSettings);
                if (userPermission != null)
                {
                    var memberId = context.User.FindFirstValue(MemberIdClaimType);
                    var workTask = context.Resource as WorkTask;
                    var taskSucceed = workTask != null;
                    var memberSucceed = !string.IsNullOrEmpty(memberId);

                    var assignToAllScope = userPermission.AssignedScopes.OfType<TaskAssignToAllScope>().FirstOrDefault();
                    var assignToMyOrganizationScope = userPermission.AssignedScopes.OfType<TaskAssignToMyOrganizationScope>().FirstOrDefault();
                    if (taskSucceed && assignToAllScope != null)
                    {
                        context.Succeed(requirement);
                    }
                    else if (taskSucceed && memberSucceed && assignToMyOrganizationScope != null)
                    {
                        var member = await _memberService.GetByIdAsync(memberId);
                        var organizationId = member?.MemberType switch
                        {
                            nameof(Contact) => (member as Contact)?.Organizations?.FirstOrDefault(),
                            nameof(Employee) => (member as Employee)?.Organizations?.FirstOrDefault(),
                            _ => null
                        };

                        if (!string.IsNullOrEmpty(organizationId))
                        {
                            var criteria = new MembersSearchCriteria
                            {
                                MemberId = organizationId,
                                DeepSearch = true,
                                Skip = 0,
                                Take = 1,
                                ObjectTypes = new[] { nameof(Contact), nameof(Employee) },
                                ObjectIds = new[] { workTask.ResponsibleId },
                            };
                            var assignedMember = _memberSearchService.SearchMembersAsync(criteria);

                            if (assignedMember != null)
                            {
                                context.Succeed(requirement);
                            }
                        }
                    }
                    else if (taskSucceed && memberSucceed && memberId == workTask.ResponsibleId)
                    {
                        context.Succeed(requirement);
                    }
                }
            }
        }
    }
}
