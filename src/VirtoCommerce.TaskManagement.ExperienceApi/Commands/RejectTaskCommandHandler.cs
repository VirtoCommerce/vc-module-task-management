using System.Threading;
using System.Threading.Tasks;
using VirtoCommerce.TaskManagement.Core.Models;
using VirtoCommerce.TaskManagement.Core.Services;

namespace VirtoCommerce.TaskManagement.ExperienceApi.Commands;

public class RejectTaskCommandHandler : WorkTaskCommandHandler<RejectTaskCommand>
{
    public RejectTaskCommandHandler(IWorkTaskService workTaskService)
        : base(workTaskService)
    {
    }

    public override Task<WorkTask> Handle(RejectTaskCommand request, CancellationToken cancellationToken)
    {
        return FinishTask(request.Id, completed: false);
    }
}
