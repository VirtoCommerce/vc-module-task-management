using System;
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
using VirtoCommerce.TaskManagement.Core.Extensions;
using VirtoCommerce.TaskManagement.Core.Models;
using VirtoCommerce.TaskManagement.Core.Services;
using VirtoCommerce.TaskManagement.Data.Authorization;
using TaskPermissions = VirtoCommerce.TaskManagement.Core.ModuleConstants.Security.Permissions;

namespace VirtoCommerce.TaskManagement.Web.Controllers.Api
{
    [Route("api/task-management")]
    [Authorize]
    public class TaskManagementController : Controller
    {
        private const string MemberIdClaimType = "memberId";

        private readonly IWorkTaskService _workTaskService;
        private readonly IWorkTaskSearchService _workTaskSearchService;
        private readonly IMemberService _memberService;
        private readonly IMemberSearchService _memberSearchService;
        private readonly ISettingsManager _settingsManager;
        private readonly IAuthorizationService _authorizationService;
        private readonly MvcNewtonsoftJsonOptions _jsonOptions;

        public TaskManagementController(
            IWorkTaskService workTaskService,
            IWorkTaskSearchService workTaskSearchService,
            IMemberService memberService,
            IMemberSearchService memberSearchService,
            ISettingsManager settingsManager,
            IAuthorizationService authorizationService,
            IOptions<MvcNewtonsoftJsonOptions> jsonOptions
            )
        {
            _workTaskService = workTaskService;
            _workTaskSearchService = workTaskSearchService;
            _memberService = memberService;
            _memberSearchService = memberSearchService;
            _settingsManager = settingsManager;
            _authorizationService = authorizationService;
            _jsonOptions = jsonOptions.Value;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<WorkTask>> Get(string id, [FromQuery] string respGroup = null)
        {
            var criteria = AbstractTypeFactory<WorkTaskSearchCriteria>.TryCreateInstance();
            criteria.ObjectIds = new[] { id };
            criteria.ResponseGroup = respGroup;

            var authorizationResult = await _authorizationService.AuthorizeAsync(User, criteria, new TaskAuthorizationRequirement(TaskPermissions.Read));
            if (!authorizationResult.Succeeded)
            {
                return Unauthorized();
            }

            var searchResult = await _workTaskSearchService.SearchAsync(criteria);
            return Ok(searchResult.Results.FirstOrDefault());
        }

        [HttpPost("search")]
        public async Task<ActionResult<WorkTaskSearchResult>> SearchTasks([FromBody] WorkTaskSearchCriteria criteria)
        {
            var authorizationResult = await _authorizationService.AuthorizeAsync(User, criteria, new TaskAuthorizationRequirement(TaskPermissions.Read));
            if (!authorizationResult.Succeeded)
            {
                return Unauthorized();
            }

            var result = await _workTaskSearchService.SearchAsync(criteria);
            return Ok(result);
        }

        [HttpPost("")]
        public async Task<ActionResult<WorkTask>> Create([FromBody] WorkTask workTask)
        {
            var authorizationResult = await _authorizationService.AuthorizeAsync(User, workTask, new TaskAuthorizationRequirement(TaskPermissions.Create));
            if (!authorizationResult.Succeeded)
            {
                return Unauthorized();
            }

            await _workTaskService.SaveChangesAsync(new[] { workTask });
            workTask = await _workTaskService.GetByIdAsync(workTask.Id);
            return workTask;
        }

        [HttpPut("")]
        public async Task<ActionResult<WorkTask>> Update([FromBody] WorkTask workTask)
        {
            var authorizationResult = await _authorizationService.AuthorizeAsync(User, workTask, new TaskAuthorizationRequirement(TaskPermissions.Update));
            if (!authorizationResult.Succeeded)
            {
                return Unauthorized();
            }

            await _workTaskService.SaveChangesAsync(new[] { workTask });
            return workTask;
        }

        [HttpDelete("")]
        public async Task<ActionResult> Delete([FromBody] string id)
        {
            var workTask = await _workTaskService.GetByIdAsync(id);
            var authorizationResult = await _authorizationService.AuthorizeAsync(User, workTask, new TaskAuthorizationRequirement(TaskPermissions.Delete));
            if (!authorizationResult.Succeeded)
            {
                return Unauthorized();
            }

            await _workTaskService.DeleteAsync(new[] { id });
            return NoContent();
        }

        [HttpPost("finish")]
        public async Task<ActionResult<WorkTask>> Finish([FromQuery] string id, [FromQuery] bool completed, [FromBody] JObject result)
        {
            var workTask = await _workTaskService.GetByIdAsync(id);
            var authorizationResult = await _authorizationService.AuthorizeAsync(User, workTask, new TaskAuthorizationRequirement(TaskPermissions.Finish));
            if (!authorizationResult.Succeeded)
            {
                return Unauthorized();
            }

            workTask.IsActive = false;
            workTask.Completed = completed;
            workTask.Result = result;
            await _workTaskService.SaveChangesAsync(new[] { workTask });
            return workTask;
        }

        [HttpPost("timeout")]
        [Authorize(TaskPermissions.Update)]
        public async Task<ActionResult<WorkTask>> Timeout(string id)
        {
            var workTask = await _workTaskService.TimeoutAsync(id);
            return workTask;
        }

        [HttpGet("contact/icon/{id}")]
        public async Task<ActionResult> ContactIcon(string id)
        {
            if (!HasPermission(User, TaskPermissions.Read, out _))
            {
                return Unauthorized();
            }

            var contact = await _memberService.GetByIdAsync(id, MemberResponseGroup.Default.ToString());

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
        public async Task<ActionResult<TaskMemberSearchResult>> SearchAssignMembers([FromBody] MembersSearchCriteria criteria)
        {
            if (!HasPermission(User, TaskPermissions.Create, out var permission) || criteria == null)
            {
                return Unauthorized();
            }

            var result = AbstractTypeFactory<TaskMemberSearchResult>.TryCreateInstance();
            result.Results = new List<TaskMember>();

            if (!User.HasGlobalPermission(TaskPermissions.Create) && !await ApplyScopeAsync(permission, criteria))
            {
                return Ok(result);
            }

            criteria.ResponseGroup = MemberResponseGroup.Default.ToString();

            var searchResult = await _memberSearchService.SearchMembersAsync(criteria);

            result.TotalCount = searchResult.TotalCount;
            result.Results = searchResult.Results.Select(ToTaskMember).ToList();

            return Ok(result);
        }

        [HttpGet("members/{id}")]
        public async Task<ActionResult<TaskMember>> GetMemberById([FromRoute] string id)
        {
            // The member is looked up to pick a responsible person for a task, the same as members/search
            if (!HasPermission(User, TaskPermissions.Create, out var permission))
            {
                return Unauthorized();
            }

            var member = await _memberService.GetByIdAsync(id, MemberResponseGroup.Default.ToString());

            if (member == null)
            {
                return Ok(null);
            }

            if (!User.HasGlobalPermission(TaskPermissions.Create) && !await CanSeeMemberAsync(permission, member))
            {
                return Unauthorized();
            }

            return Ok(ToTaskMember(member));
        }

        [HttpGet("tasks/types")]
        public async Task<ActionResult<IEnumerable<TaskType>>> GetTaskTypes()
        {
            var setting = await _settingsManager.GetObjectSettingAsync(ModuleConstants.Settings.General.TaskTypes.Name);
            var result = setting.AllowedValues.Select(x => new TaskType { Name = x as string }).ToList();

            return Ok(result);
        }

        private static TaskMember ToTaskMember(Member member)
        {
            return AbstractTypeFactory<TaskMember>.TryCreateInstance().FromModel(member);
        }

        // A scoped permission sees the user's own member, or with the organization scope their organization and its members.
        // Returns false when the scope leaves nothing to search.
        private async Task<bool> ApplyScopeAsync(Permission permission, MembersSearchCriteria criteria)
        {
            var currentMemberId = User.FindFirstValue(MemberIdClaimType);
            if (string.IsNullOrEmpty(currentMemberId))
            {
                return false;
            }

            var organizationId = await GetScopedOrganizationIdAsync(permission, currentMemberId);
            if (!string.IsNullOrEmpty(organizationId))
            {
                criteria.MemberId = organizationId;
                return true;
            }

            if (!permission.AssignedScopes.OfType<TaskToMeScope>().Any())
            {
                return false;
            }

            if (!criteria.ObjectIds.IsNullOrEmpty() && !criteria.ObjectIds.Contains(currentMemberId, StringComparer.OrdinalIgnoreCase))
            {
                return false;
            }

            criteria.ObjectIds = [currentMemberId];
            return true;
        }

        private async Task<bool> CanSeeMemberAsync(Permission permission, Member member)
        {
            var currentMemberId = User.FindFirstValue(MemberIdClaimType);
            if (string.IsNullOrEmpty(currentMemberId))
            {
                return false;
            }

            if (member.Id.EqualsIgnoreCase(currentMemberId))
            {
                return true;
            }

            var organizationId = await GetScopedOrganizationIdAsync(permission, currentMemberId);

            return !string.IsNullOrEmpty(organizationId) &&
                (member.Id.EqualsIgnoreCase(organizationId) || organizationId.EqualsIgnoreCase(member.GetMemberOrganizationId()));
        }

        private async Task<string> GetScopedOrganizationIdAsync(Permission permission, string currentMemberId)
        {
            if (!permission.AssignedScopes.OfType<TaskToMyOrganizationScope>().Any())
            {
                return null;
            }

            var currentMember = await _memberService.GetByIdAsync(currentMemberId, MemberResponseGroup.Default.ToString());

            return currentMember.GetMemberOrganizationId();
        }

        private bool HasPermission(ClaimsPrincipal user, string permissionName, out Permission permission)
        {
            permission = user.FindPermission(permissionName, _jsonOptions.SerializerSettings);
            return user.HasGlobalPermission(permissionName) || permission != null;
        }
    }
}
