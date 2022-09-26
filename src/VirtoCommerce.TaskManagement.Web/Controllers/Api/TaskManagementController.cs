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

        // GET: api/task-management
        /// <summary>
        /// Get message
        /// </summary>
        /// <remarks>Return "Hello world!" message</remarks>
        [HttpGet]
        [Route("")]
        [Authorize(ModuleConstants.Security.Permissions.Read)]
        public async Task<ActionResult<WorkTask>> Get(string id)
        {
            var workTask = await _workTaskService.GetByIdAsync(id);
            return workTask;
        }

        [HttpPost]
        [Route("")]
        [Authorize(ModuleConstants.Security.Permissions.Create)]
        public async Task<ActionResult<WorkTask>> Create([FromBody] WorkTask workTask)
        {
            await _workTaskService.SaveChangesAsync(new[] { workTask });
            return workTask;
        }

        [HttpPost]
        [Route("complete")]
        [Authorize(ModuleConstants.Security.Permissions.Update)]
        public async Task<ActionResult<WorkTask>> Complete(string id, [FromBody] JObject result)
        {
            var workTask = await _workTaskService.CompleteAsync(id, result);
            return workTask;
        }

        [HttpPost]
        [Route("cancel")]
        [Authorize(ModuleConstants.Security.Permissions.Update)]
        public async Task<ActionResult<WorkTask>> Cancel(string id, [FromBody] JObject result)
        {
            var workTask = await _workTaskService.CancelAsync(id, result);
            return workTask;
        }
    }
}
