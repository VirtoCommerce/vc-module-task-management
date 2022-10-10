using System.Threading.Tasks;
using GraphQL;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using VirtoCommerce.TaskManagement.Core.Models;
using VirtoCommerce.TaskManagement.Core.Services;
using VirtoCommerce.TaskManagement.ExperienceApi.Schemas;

namespace VirtoCommerce.TaskManagement.ExperienceApi.Commands;

public class ConfirmTaskCommandBuilder : WorkTaskCommandBuilder<ConfirmTaskCommand, WorkTask, ConfirmTaskCommandType, WorkTaskType>
{
    protected override string Name => "confirmTask";

    public ConfirmTaskCommandBuilder(IMediator mediator, IAuthorizationService authorizationService, IWorkTaskService workTaskService)
        : base(mediator, authorizationService, workTaskService)
    {
    }

    protected override async Task BeforeMediatorSend(IResolveFieldContext<object> context, ConfirmTaskCommand request)
    {
        await base.BeforeMediatorSend(context, request);
        await Authorize(context, request.Id);
    }
}
