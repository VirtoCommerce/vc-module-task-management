using System.Threading.Tasks;
using GraphQL;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using VirtoCommerce.TaskManagement.Core.Models;
using VirtoCommerce.TaskManagement.Core.Services;
using VirtoCommerce.TaskManagement.ExperienceApi.Schemas;

namespace VirtoCommerce.TaskManagement.ExperienceApi.Commands;

public class RejectTaskCommandBuilder : WorkTaskCommandBuilder<RejectTaskCommand, WorkTask, RejectTaskCommandType, WorkTaskType>
{
    protected override string Name => "rejectTask";

    public RejectTaskCommandBuilder(IMediator mediator, IAuthorizationService authorizationService, IWorkTaskService workTaskService)
        : base(mediator, authorizationService, workTaskService)
    {
    }

    protected override async Task BeforeMediatorSend(IResolveFieldContext<object> context, RejectTaskCommand request)
    {
        await base.BeforeMediatorSend(context, request);
        await Authorize(context, request.Id);
    }
}
