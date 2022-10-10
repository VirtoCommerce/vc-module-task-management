using System.Threading;
using System.Threading.Tasks;
using VirtoCommerce.TaskManagement.Core.Models;
using VirtoCommerce.TaskManagement.Core.Services;

namespace VirtoCommerce.TaskManagement.ExperienceApi.Commands;

public class ConfirmTaskCommandHandler : WorkTaskCommandHandler<ConfirmTaskCommand>
{
    public ConfirmTaskCommandHandler(IWorkTaskService workTaskService)
        : base(workTaskService)
    {
    }

    public override Task<WorkTask> Handle(ConfirmTaskCommand request, CancellationToken cancellationToken)
    {
        return FinishTask(request.Id, completed: true);
    }
}
