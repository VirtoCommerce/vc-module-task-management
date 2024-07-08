using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using VirtoCommerce.CustomerModule.Core.Extensions;
using VirtoCommerce.CustomerModule.Core.Model;
using VirtoCommerce.CustomerModule.Core.Services;
using VirtoCommerce.Platform.Core.Security;
using VirtoCommerce.Platform.Security.Authorization;
using VirtoCommerce.TaskManagement.Core.Extensions;
using VirtoCommerce.TaskManagement.Core.Models;
using VirtoCommerce.TaskManagement.Data.Authorization;

namespace VirtoCommerce.TaskManagement.Web.Authorization
{
    public sealed class TaskAuthorizationHandler : PermissionAuthorizationHandlerBase<TaskAuthorizationRequirement>
    {
        private const string MemberIdClaimType = "memberId";

        private readonly MvcNewtonsoftJsonOptions _jsonOptions;
        private readonly IMemberService _memberService;

        public TaskAuthorizationHandler(
            IOptions<MvcNewtonsoftJsonOptions> jsonOptions,
            IMemberService memberService
        )
        {
            _jsonOptions = jsonOptions.Value;
            _memberService = memberService;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, TaskAuthorizationRequirement requirement)
        {
            await base.HandleRequirementAsync(context, requirement);

            var currentMemberId = context.User.FindFirstValue(MemberIdClaimType);
            var currentMemberOrganizationId = context.User.GetCurrentOrganizationId();
            var permission = context.User.FindPermission(requirement.Permission, _jsonOptions.SerializerSettings);

            if (context.HasSucceeded)
            {
                switch (context.Resource)
                {
                    case WorkTask workTask:
                        if (!context.User.HasGlobalPermission(requirement.Permission))
                        {
                            var responsibleMember = await _memberService.GetByIdAsync(workTask.ResponsibleId);
                            // todo: get organization id from another member
                            var organizationId = responsibleMember.GetMemberOrganizationId();
                            SetResponsible(workTask, responsibleMember, organizationId);
                            context.Succeed(requirement);
                        }
                        break;
                    case WorkTaskSearchCriteria criteria when criteria.OnlyAssignedToMe == true:
                        {
                            criteria.ResponsibleIds = new[] { currentMemberId };
                            context.Succeed(requirement);
                            break;
                        }
                }
            }
            else if (permission != null)
            {
                var taskToMeScope = permission.AssignedScopes.OfType<TaskToMeScope>().FirstOrDefault();
                var taskToMyOrganizationScope = permission.AssignedScopes.OfType<TaskToMyOrganizationScope>().FirstOrDefault();

                switch (context.Resource)
                {
                    case WorkTask workTask when taskToMyOrganizationScope != null:
                        {
                            var responsibleMember = await _memberService.GetByIdAsync(workTask.ResponsibleId);
                            // todo: get organization id from another member
                            var organizationId = responsibleMember.GetMemberOrganizationId();

                            if (currentMemberOrganizationId == organizationId)
                            {
                                SetResponsible(workTask, responsibleMember, organizationId);
                                context.Succeed(requirement);
                            }

                            break;
                        }
                    case WorkTask workTask when taskToMeScope != null && workTask.ResponsibleId == currentMemberId:
                        {
                            var responsibleMember = await _memberService.GetByIdAsync(workTask.ResponsibleId);
                            // todo: get organization id from another member
                            var organizationId = responsibleMember.GetMemberOrganizationId();
                            SetResponsible(workTask, responsibleMember, organizationId);
                            context.Succeed(requirement);
                            break;
                        }
                    case WorkTaskSearchCriteria criteria:
                        {
                            var isSearchOrganizationTasks = taskToMyOrganizationScope != null;
                            SetCriteria(criteria, isSearchOrganizationTasks, currentMemberId, currentMemberOrganizationId);
                            context.Succeed(requirement);
                            break;
                        }
                }
            }
        }

        private static void SetCriteria(WorkTaskSearchCriteria criteria, bool isSearchOrganizationTasks, string memberId, string organizationId)
        {
            if (!string.IsNullOrEmpty(organizationId) && isSearchOrganizationTasks && !(criteria.OnlyAssignedToMe ?? false))
            {
                criteria.OrganizationIds = new[] { organizationId };
            }
            else
            {
                criteria.ResponsibleIds = new[] { memberId };
            }
        }

        private static void SetResponsible(WorkTask workTask, Member member, string organizationId)
        {
            workTask.ResponsibleName = member?.Name;
            workTask.OrganizationId = organizationId;
        }
    }
}
