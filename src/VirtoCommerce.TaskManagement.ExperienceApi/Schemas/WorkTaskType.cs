using GraphQL.Types;
using VirtoCommerce.ExperienceApiModule.Core.Schemas;
using VirtoCommerce.TaskManagement.Core.Models;

namespace VirtoCommerce.TaskManagement.ExperienceApi.Schemas;

public class WorkTaskType : ExtendableGraphType<WorkTask>
{
    public WorkTaskType()
    {
        Field(x => x.Completed, nullable: true);
        Field(x => x.CreatedBy, nullable: true);
        Field(x => x.CreatedDate, nullable: false);
        Field(x => x.Description, nullable: true);
        Field(x => x.DueDate, nullable: true);
        Field(x => x.Id, nullable: false);
        Field(x => x.IsActive, nullable: false);
        Field(x => x.ModifiedBy, nullable: true);
        Field(x => x.ModifiedDate, nullable: true);
        Field<IntGraphType>(nameof(WorkTask.Priority), resolve: context => (int)context.Source.Priority);
        Field(x => x.ResponsibleName, nullable: true);
        Field(x => x.StoreId, nullable: true);
        Field(x => x.Type, nullable: true);
        Field(x => x.WorkflowId, nullable: true);
    }
}
