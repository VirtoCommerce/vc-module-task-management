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
using VirtoCommerce.Platform.Core.Security;
using VirtoCommerce.TaskManagement.Core;
using VirtoCommerce.TaskManagement.Core.Models;
using VirtoCommerce.TaskManagement.Core.Services;
using VirtoCommerce.TaskManagement.Data.Authorization;

namespace VirtoCommerce.TaskManagement.Web.Controllers.Api
{
    [Route("api/task-management")]
    public class TaskManagementController : Controller
    {
        private readonly IWorkTaskService _workTaskService;
        private readonly IWorkTaskSearchService _workTaskSearchService;
        private readonly IMemberService _memberService;
        private readonly IMemberSearchService _memberSearchService;
        private readonly IAuthorizationService _authorizationService;
        private readonly MvcNewtonsoftJsonOptions _jsonOptions;

        public TaskManagementController(
            IWorkTaskService workTaskService,
            IWorkTaskSearchService workTaskSearchService,
            IMemberService memberService,
            IMemberSearchService memberSearchService,
            IAuthorizationService authorizationService,
            IOptions<MvcNewtonsoftJsonOptions> jsonOptions)
        {
            _workTaskService = workTaskService;
            _workTaskSearchService = workTaskSearchService;
            _memberService = memberService;
            _memberSearchService = memberSearchService;
            _authorizationService = authorizationService;
            _jsonOptions = jsonOptions.Value;
        }

        [HttpGet("{id}")]
        [Authorize(ModuleConstants.Security.Permissions.Read)]
        public async Task<ActionResult<WorkTask>> Get(string id, [FromQuery] string respGroup = null)
        {
            var workTask = await _workTaskService.GetByIdAsync(id);
            return workTask;
        }

        [HttpPost("search")]
        [Authorize(ModuleConstants.Security.Permissions.Read)]
        public async Task<ActionResult<WorkTaskSearchResult>> SearchTasks([FromBody] WorkTaskSearchCriteria criteria)
        {
            var result = await _workTaskSearchService.SearchAsync(criteria);
            return result;
        }

        [HttpPost("")]
        public async Task<ActionResult<WorkTask>> Create([FromBody] WorkTask workTask)
        {
            var authorizationResult = await _authorizationService.AuthorizeAsync(User, workTask,
                new TaskAssignAuthorizationRequirement(ModuleConstants.Security.Permissions.Create));
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
            var authorizationResult = await _authorizationService.AuthorizeAsync(User, workTask,
                new TaskAssignAuthorizationRequirement(ModuleConstants.Security.Permissions.Update));
            if (!authorizationResult.Succeeded)
            {
                return Unauthorized();
            }

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

        [HttpPost("complete")]
        [Authorize(ModuleConstants.Security.Permissions.Update)]
        public async Task<ActionResult<WorkTask>> Complete(string id, [FromBody] JObject result)
        {
            var workTask = await _workTaskService.CompleteAsync(id, result);
            return workTask;
        }

        [HttpPost("cancel")]
        [Authorize(ModuleConstants.Security.Permissions.Update)]
        public async Task<ActionResult<WorkTask>> Cancel(string id, [FromBody] JObject result)
        {
            var workTask = await _workTaskService.CancelAsync(id, result);
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
            var contact = await _memberService.GetByIdAsync(id, memberType: "Contact");

            if (contact != null && !string.IsNullOrEmpty(contact.IconUrl))
            {
                using var client = new HttpClient();
                using var response = await client.GetAsync(contact.IconUrl);
                await using var stream = await response.Content.ReadAsStreamAsync();
                var contentTypeHeader = response.Content.Headers.FirstOrDefault(h => h.Key == "Content-Type").Value
                    .FirstOrDefault();

                var memoryStream = new MemoryStream();
                stream.CopyTo(memoryStream);
                memoryStream.Position = 0;
                return File(memoryStream, contentTypeHeader);
            }

            return NotFound();
        }

        [HttpPost("members/search")]
        public async Task<ActionResult<MemberSearchResult>> SearchAssignMembers(
            [FromBody] MembersSearchCriteria criteria)
        {
            MemberSearchResult result = null;
            var userPermission = User.FindPermission(ModuleConstants.Security.Permissions.Create,
                _jsonOptions.SerializerSettings);

            if (userPermission == null)
            {
                return Unauthorized();
            }

            if (criteria?.ObjectIds?.Any() == true)
            {
                result = await _memberSearchService.SearchMembersAsync(criteria);
            }
            else
            {
                var assignToAllScope = userPermission.AssignedScopes.OfType<TaskAssignToAllScope>().FirstOrDefault();
                var assignToMyOrganizationScope = userPermission.AssignedScopes
                    .OfType<TaskAssignToMyOrganizationScope>().FirstOrDefault();

                var memberId = User.FindFirstValue("memberId");
                var memberExist = !string.IsNullOrEmpty(memberId);
                string organizationId = null;

                if (memberExist)
                {
                    var member = await _memberService.GetByIdAsync(memberId);
                    memberExist = member != null;

                    organizationId = member?.MemberType switch
                    {
                        nameof(Contact) => (member as Contact)?.Organizations?.FirstOrDefault(),
                        nameof(Employee) => (member as Employee)?.Organizations?.FirstOrDefault(),
                        _ => null
                    };
                }

                var organizationExist = !string.IsNullOrEmpty(organizationId);

                if (assignToAllScope != null)
                {
                    result = await _memberSearchService.SearchMembersAsync(criteria);
                }
                else if (organizationExist && assignToMyOrganizationScope != null)
                {
                    criteria.MemberId = organizationId;
                    result = await _memberSearchService.SearchMembersAsync(criteria);
                }
                else if (memberExist)
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
    }
}
