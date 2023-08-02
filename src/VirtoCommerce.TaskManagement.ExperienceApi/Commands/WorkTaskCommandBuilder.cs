using System.Threading.Tasks;
using GraphQL;
using GraphQL.Types;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using VirtoCommerce.ExperienceApiModule.Core.BaseQueries;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.TaskManagement.Core.Services;
using VirtoCommerce.TaskManagement.ExperienceApi.Authorization;

namespace VirtoCommerce.TaskManagement.ExperienceApi.Commands;

public abstract class WorkTaskCommandBuilder<TCommand, TResult, TCommandGraphType, TResultGraphType>
    : CommandBuilder<TCommand, TResult, TCommandGraphType, TResultGraphType>
    where TCommand : IRequest<TResult>
    where TCommandGraphType : IInputObjectGraphType
    where TResultGraphType : IGraphType
{
    private readonly IWorkTaskService _workTaskService;

    protected WorkTaskCommandBuilder(
        IMediator mediator,
        IAuthorizationService authorizationService,
        IWorkTaskService workTaskService)
        : base(mediator, authorizationService)
    {
        _workTaskService = workTaskService;
    }

    protected async Task Authorize(IResolveFieldContext<object> context, string taskId)
    {
        var task = await _workTaskService.GetByIdAsync(taskId);
        await Authorize(context, task, new WorkTaskAuthorizationRequirement());
    }
}
