using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using VirtoCommerce.Platform.Core;
using VirtoCommerce.TaskManagement.Core.Models;
using VirtoCommerce.TaskManagement.ExperienceApi.Queries;

namespace VirtoCommerce.TaskManagement.ExperienceApi.Authorization;

public class WorkTaskAuthorizationRequirement : IAuthorizationRequirement
{
}

public class WorkTaskAuthorizationHandler : AuthorizationHandler<WorkTaskAuthorizationRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, WorkTaskAuthorizationRequirement requirement)
    {
        var result = context.User.IsInRole(PlatformConstants.Security.SystemRoles.Administrator);

        if (!result)
        {
            var currentUserName = context.User.Identity?.Name;

            switch (context.Resource)
            {
                case WorkTask task:
                    result = currentUserName != null && task.ResponsibleName == currentUserName;
                    break;
                case WorkTasksQuery query:
                    result = currentUserName != null;
                    query.ResponsibleName = currentUserName;
                    break;
            }
        }

        if (result)
        {
            context.Succeed(requirement);
        }
        else
        {
            context.Fail();
        }

        return Task.CompletedTask;
    }
}
