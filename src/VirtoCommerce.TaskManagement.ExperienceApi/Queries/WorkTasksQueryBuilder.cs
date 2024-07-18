using System.Threading.Tasks;
using GraphQL;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using VirtoCommerce.Xapi.Core.BaseQueries;
using VirtoCommerce.TaskManagement.Core.Models;
using VirtoCommerce.TaskManagement.ExperienceApi.Authorization;
using VirtoCommerce.TaskManagement.ExperienceApi.Schemas;

namespace VirtoCommerce.TaskManagement.ExperienceApi.Queries;

public class WorkTasksQueryBuilder : SearchQueryBuilder<WorkTasksQuery, WorkTaskSearchResult, WorkTask, WorkTaskType>
{
    protected override string Name => "tasks";

    public WorkTasksQueryBuilder(IMediator mediator, IAuthorizationService authorizationService)
        : base(mediator, authorizationService)
    {
    }

    protected override async Task BeforeMediatorSend(IResolveFieldContext<object> context, WorkTasksQuery request)
    {
        await base.BeforeMediatorSend(context, request);
        await Authorize(context, request, new WorkTaskAuthorizationRequirement());
    }
}
