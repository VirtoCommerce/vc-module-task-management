using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using VirtoCommerce.CustomerModule.Core.Model;
using VirtoCommerce.CustomerModule.Core.Model.Search;
using VirtoCommerce.CustomerModule.Core.Services;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Security;
using VirtoCommerce.Platform.Core.Settings;
using VirtoCommerce.TaskManagement.Core;
using VirtoCommerce.TaskManagement.Core.Models;
using VirtoCommerce.TaskManagement.Core.Services;
using VirtoCommerce.TaskManagement.Data.Authorization;

namespace VirtoCommerce.TaskManagement.Web.Controllers.Api
{
    [Route("api/task-management")]
    public class TaskManagementController : Controller
    {
        private const string MemberIdClaimType = "memberId";

        private readonly IWorkTaskService _workTaskService;
        private readonly IWorkTaskSearchService _workTaskSearchService;
        private readonly IMemberService _memberService;
        private readonly IMemberSearchService _memberSearchService;
        private readonly IAuthorizationService _authorizationService;
        private readonly ISettingsManager _settingsManager;
        private readonly MvcNewtonsoftJsonOptions _jsonOptions;

        public TaskManagementController(
            IWorkTaskService workTaskService,
            IWorkTaskSearchService workTaskSearchService,
            IMemberService memberService,
            IMemberSearchService memberSearchService,
            IAuthorizationService authorizationService,
            ISettingsManager settingsManager,
            IOptions<MvcNewtonsoftJsonOptions> jsonOptions
            )
        {
            _workTaskService = workTaskService;
            _workTaskSearchService = workTaskSearchService;
            _memberService = memberService;
            _memberSearchService = memberSearchService;
            _authorizationService = authorizationService;
            _settingsManager = settingsManager;
            _jsonOptions = jsonOptions.Value;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<WorkTask>> Get(string id, [FromQuery] string respGroup = null)
        {
            var userPermission = User.FindPermission(ModuleConstants.Security.Permissions.Read, _jsonOptions.SerializerSettings);
            if (!User.HasGlobalPermission(ModuleConstants.Security.Permissions.Read) && userPermission == null)
            {
                return Unauthorized();
            }

            var criteria = AbstractTypeFactory<WorkTaskSearchCriteria>.TryCreateInstance();
            criteria.ObjectIds = new[] { id };
            criteria.ResponseGroup = respGroup;

            await FillCriteriaResponsibleInformation(criteria, userPermission);

            var searchResult = await _workTaskSearchService.SearchAsync(criteria);
            return Ok(searchResult.Results.FirstOrDefault());
        }

        [HttpPost("search")]
        public async Task<ActionResult<WorkTaskSearchResult>> SearchTasks([FromBody] WorkTaskSearchCriteria criteria)
        {
            if (!HasPermission(User, ModuleConstants.Security.Permissions.Read, out var userPermission))
            {
                return Unauthorized();
            }

            await FillCriteriaResponsibleInformation(criteria, userPermission);
            var result = await _workTaskSearchService.SearchAsync(criteria);
            return Ok(result);
        }

        [HttpPost("")]
        public async Task<ActionResult<WorkTask>> Create([FromBody] WorkTask workTask)
        {
            if (!HasPermission(User, ModuleConstants.Security.Permissions.Create, out var userPermission))
            {
                return Unauthorized();
            }

            await FillWorkTaskResponsibleInformation(workTask);
            await _workTaskService.SaveChangesAsync(new[] { workTask });
            workTask = await _workTaskService.GetByIdAsync(workTask.Id);
            return workTask;
        }

        [HttpPut("")]
        public async Task<ActionResult<WorkTask>> Update([FromBody] WorkTask workTask)
        {
            if (!HasPermission(User, ModuleConstants.Security.Permissions.Update, out var userPermission))
            {
                return Unauthorized();
            }

            await FillWorkTaskResponsibleInformation(workTask);
            await _workTaskService.SaveChangesAsync(new[] { workTask });
            return workTask;
        }

        [HttpDelete("")]
        [Authorize(ModuleConstants.Security.Permissions.Delete)]
        public async Task<ActionResult> Delete([FromBody] string id)
        {
            await _workTaskService.DeleteAsync(new[] { id });
            return NoContent();
        }

        [HttpPost("finish")]
        [Authorize(ModuleConstants.Security.Permissions.Finish)]
        public async Task<ActionResult<WorkTask>> Finish([FromQuery] string id, [FromQuery] bool completed, [FromBody] JObject result)
        {
            var workTask = await _workTaskService.FinishAsync(id, completed, result);
            return workTask;
        }

        [HttpPost("timeout")]
        [Authorize(ModuleConstants.Security.Permissions.Update)]
        public async Task<ActionResult<WorkTask>> Timeout(string id)
        {
            var workTask = await _workTaskService.TimeoutAsync(id);
            return workTask;
        }

        [HttpGet("contact/icon/{id}")]
        public async Task<ActionResult> ContactIcon(string id)
        {
            var contact = await _memberService.GetByIdAsync(id);

            if (contact != null && !string.IsNullOrEmpty(contact.IconUrl))
            {
                using var client = new HttpClient();
                using var response = await client.GetAsync(contact.IconUrl);
                await using var stream = await response.Content.ReadAsStreamAsync();
                var contentTypeHeader = response.Content.Headers.FirstOrDefault(h => h.Key == "Content-Type").Value.FirstOrDefault();

                var memoryStream = new MemoryStream();
                stream.CopyTo(memoryStream);
                memoryStream.Position = 0;
                return File(memoryStream, contentTypeHeader);
            }

            return NotFound();
        }

        [HttpPost("members/search")]
        public async Task<ActionResult<MemberSearchResult>> SearchAssignMembers([FromBody] MembersSearchCriteria criteria)
        {
            MemberSearchResult result = null;
            if (!HasPermission(User, ModuleConstants.Security.Permissions.Create, out var userPermission))
            {
                return Unauthorized();
            }

            if (criteria?.ObjectIds?.Any() == true)
            {
                result = await _memberSearchService.SearchMembersAsync(criteria);
            }
            else
            {
                var assignToMeScope = userPermission?.AssignedScopes.OfType<TaskAssignToMeScope>().FirstOrDefault();
                var assignToMyOrganizationScope = userPermission?.AssignedScopes.OfType<TaskAssignToMyOrganizationScope>().FirstOrDefault();

                var memberId = User.FindFirstValue(MemberIdClaimType);
                var member = await _memberService.GetByIdAsync(memberId);
                var organizationId = GetMemberOrganizationId(member);

                if (userPermission == null || !userPermission.AssignedScopes.Any())
                {
                    result = await _memberSearchService.SearchMembersAsync(criteria);
                }
                else if (!string.IsNullOrEmpty(organizationId) && assignToMyOrganizationScope != null)
                {
                    criteria.MemberId = organizationId;
                    result = await _memberSearchService.SearchMembersAsync(criteria);
                }
                else if (assignToMeScope != null && !string.IsNullOrEmpty(memberId))
                {
                    criteria.ObjectIds = new[] { memberId };
                    result = await _memberSearchService.SearchMembersAsync(criteria);
                }
            }

            return Ok(result);
        }

        [HttpGet("members/{id}")]
        public async Task<ActionResult<Member>> GetMemberById([FromRoute] string id)
        {
            var member = await _memberService.GetByIdAsync(id);
            return Ok(member);
        }

        [HttpGet("tasks/types")]
        public async Task<ActionResult<IEnumerable<TaskType>>> GetTaskTypes()
        {
            var setting = await _settingsManager.GetObjectSettingAsync(ModuleConstants.Settings.General.TaskTypes.Name);
            var result = setting.AllowedValues.Select(x => new TaskType { Name = x as string }).ToList();

            return Ok(result);
        }

        private async Task FillWorkTaskResponsibleInformation(WorkTask workTask)
        {
            if (!string.IsNullOrEmpty(workTask.ResponsibleId))
            {
                var member = await _memberService.GetByIdAsync(workTask.ResponsibleId);
                var organizationId = GetMemberOrganizationId(member);

                workTask.ResponsibleName = member?.Name;
                workTask.OrganizationId = organizationId;
            }
        }

        private async Task FillCriteriaResponsibleInformation(WorkTaskSearchCriteria criteria, Permission permission)
        {
            var memberId = User.FindFirstValue(MemberIdClaimType);
            if (criteria.OnlyAssignedToMe == true)
            {
                criteria.ResponsibleIds = new[] { memberId };
            }
            else
            {
                var assignToMeScope = permission?.AssignedScopes.OfType<TaskAssignToMeScope>().FirstOrDefault();
                var assignToMyOrganizationScope = permission?.AssignedScopes.OfType<TaskAssignToMyOrganizationScope>()
                    .FirstOrDefault();

                var member = await _memberService.GetByIdAsync(memberId);
                var organizationId = GetMemberOrganizationId(member);

                if (!string.IsNullOrEmpty(organizationId) && assignToMyOrganizationScope != null)
                {
                    criteria.OrganizationIds = new[] { organizationId };
                }
                else if ((assignToMeScope != null || assignToMyOrganizationScope != null) &&
                         !string.IsNullOrEmpty(memberId))
                {
                    criteria.ResponsibleIds = new[] { memberId };
                }
            }
        }

        private static string GetMemberOrganizationId(Member member)
        {
            return member?.MemberType switch
            {
                nameof(Contact) => (member as Contact)?.Organizations?.FirstOrDefault(),
                nameof(Employee) => (member as Employee)?.Organizations?.FirstOrDefault(),
                _ => null
            };
        }

        private bool HasPermission(ClaimsPrincipal user, string permission, out Permission userPermission)
        {
            userPermission = user.FindPermission(permission, _jsonOptions.SerializerSettings);
            return !user.HasGlobalPermission(permission) && userPermission == null;
        }
    }
}
