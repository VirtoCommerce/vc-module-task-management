using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using VirtoCommerce.TaskManagement.Core;
using VirtoCommerce.TaskManagement.Core.Models;

namespace VirtoCommerce.TaskManagement.Web.Controllers.Api
{
    [Route("api/task-management")]
    public class TaskManagementController : Controller
    {
        // GET: api/task-management
        /// <summary>
        /// Get message
        /// </summary>
        /// <remarks>Return "Hello world!" message</remarks>
        [HttpGet]
        [Route("")]
        [Authorize(ModuleConstants.Security.Permissions.Read)]
        public ActionResult<Task> Get(string id)
        {
            dynamic jsonObject = new JObject();
            jsonObject["Create-Date"] = DateTime.Now; //<-Index use
            jsonObject.Album = "Me Against the world"; //<- Property use
            jsonObject["Create-Year"] = 1995; //<-Index use
            jsonObject.Artist = "2Pac"; //<-Property use

            return new Task { Id = "Test", Type = "QuoteApprove", IsCompleted = false, Description = "Please review the quote proposal.", Parameters = jsonObject };
        }

        [HttpPost]
        [Route("complete")]
        [Authorize(ModuleConstants.Security.Permissions.Read)]
        public ActionResult<Task> Complete(string id, [FromBody] JObject result)
        {
            dynamic jsonObject = new JObject();
            jsonObject["Create-Date"] = DateTime.Now; //<-Index use
            jsonObject.Album = "Me Against the world"; //<- Property use
            jsonObject["Create-Year"] = 1995; //<-Index use
            jsonObject.Artist = "2Pac"; //<-Property use

            return new Task { Id = "Test", Type = "QuoteApprove", IsCompleted = true, Description = "Please review the quote proposal.", Parameters = jsonObject, Result = result };
        }
    }
}
