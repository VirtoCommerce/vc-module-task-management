using GraphQL.Types;
using VirtoCommerce.TaskManagement.Core.Models;
using VirtoCommerce.Xapi.Core.Infrastructure;

namespace VirtoCommerce.TaskManagement.ExperienceApi.Commands;

public class ConfirmTaskCommand : ICommand<WorkTask>
{
    public string Id { get; set; }
}

public class ConfirmTaskCommandType : InputObjectGraphType<ConfirmTaskCommand>
{
    public ConfirmTaskCommandType()
    {
        Field(x => x.Id);
    }
}
