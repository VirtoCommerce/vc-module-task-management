using GraphQL.Types;
using VirtoCommerce.Xapi.Core.Infrastructure;
using VirtoCommerce.TaskManagement.Core.Models;

namespace VirtoCommerce.TaskManagement.ExperienceApi.Commands;

public class RejectTaskCommand : ICommand<WorkTask>
{
    public string Id { get; set; }
}

public class RejectTaskCommandType : InputObjectGraphType<RejectTaskCommand>
{
    public RejectTaskCommandType()
    {
        Field(x => x.Id);
    }
}
