using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using VirtoCommerce.TaskManagement.Core;
using VirtoCommerce.TaskManagement.Core.Models;
using VirtoCommerce.TaskManagement.Core.Services;

namespace VirtoCommerce.TaskManagement.Web.Controllers.Api
{
    [Route("api/task-management")]
    public class TaskManagementController : Controller
    {
        private readonly IWorkTaskService _workTaskService;
        private readonly IWorkTaskSearchService _workTaskSearchService;

        public TaskManagementController(IWorkTaskService workTaskService, IWorkTaskSearchService workTaskSearchService)
        {
            _workTaskService = workTaskService;
            _workTaskSearchService = workTaskSearchService;
        }

        [HttpGet("")]
        [Authorize(ModuleConstants.Security.Permissions.Read)]
        public async Task<ActionResult<WorkTask>> Get(string id)
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
        [Authorize(ModuleConstants.Security.Permissions.Create)]
        public async Task<ActionResult<WorkTask>> Create([FromBody] WorkTask workTask)
        {
            await _workTaskService.SaveChangesAsync(new[] { workTask });
            workTask = await _workTaskService.GetByIdAsync(workTask.Id);
            return workTask;
        }

        [HttpPut("")]
        [Authorize(ModuleConstants.Security.Permissions.Update)]
        public async Task<ActionResult<WorkTask>> Update([FromBody] WorkTask workTask)
        {
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
    }
}
