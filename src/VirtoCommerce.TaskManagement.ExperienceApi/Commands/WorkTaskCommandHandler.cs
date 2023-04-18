using System.Threading;
using System.Threading.Tasks;
using GraphQL;
using MediatR;
using VirtoCommerce.ExperienceApiModule.Core.Helpers;
using VirtoCommerce.TaskManagement.Core.Models;
using VirtoCommerce.TaskManagement.Core.Services;

namespace VirtoCommerce.TaskManagement.ExperienceApi.Commands;

public abstract class WorkTaskCommandHandler<TCommand> : IRequestHandler<TCommand, WorkTask>
    where TCommand : IRequest<WorkTask>
{
    private readonly IWorkTaskService _workTaskService;

    protected WorkTaskCommandHandler(IWorkTaskService workTaskService)
    {
        _workTaskService = workTaskService;
    }

    public abstract Task<WorkTask> Handle(TCommand request, CancellationToken cancellationToken);

    protected async Task<WorkTask> FinishTask(string taskId, bool completed)
    {
        var task = await _workTaskService.GetByIdAsync(taskId);

        if (task == null)
        {
            return null;
        }

        if (!task.IsActive)
        {
            throw new ExecutionError("Work task is not active") { Code = Constants.ValidationErrorCode };
        }

        await _workTaskService.FinishAsync(task.Id, completed, result: null);

        return task;
    }
}
