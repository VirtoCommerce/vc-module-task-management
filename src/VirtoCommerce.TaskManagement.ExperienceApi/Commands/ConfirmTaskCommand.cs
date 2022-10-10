using GraphQL.Types;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;
using VirtoCommerce.TaskManagement.Core.Models;

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
