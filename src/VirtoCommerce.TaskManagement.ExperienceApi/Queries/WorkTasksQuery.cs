using System;
using System.Collections.Generic;
using GraphQL;
using GraphQL.Types;
using VirtoCommerce.ExperienceApiModule.Core.BaseQueries;
using VirtoCommerce.TaskManagement.Core.Models;

namespace VirtoCommerce.TaskManagement.ExperienceApi.Queries;

public class WorkTasksQuery : SearchQuery<WorkTaskSearchResult>
{
    public string ResponsibleName { get; set; }
    public string StoreId { get; set; }
    public DateTime? StartDueDate { get; set; }
    public DateTime? EndDueDate { get; set; }
    public bool? IsActive { get; set; }
    public bool? Completed { get; set; }

    public override IEnumerable<QueryArgument> GetArguments()
    {
        foreach (var argument in base.GetArguments())
        {
            yield return argument;
        }

        yield return Argument<StringGraphType>(nameof(ResponsibleName));
        yield return Argument<StringGraphType>(nameof(StoreId));
        yield return Argument<DateTimeGraphType>(nameof(StartDueDate));
        yield return Argument<DateTimeGraphType>(nameof(EndDueDate));
        yield return Argument<BooleanGraphType>(nameof(IsActive));
        yield return Argument<BooleanGraphType>(nameof(Completed));
    }

    public override void Map(IResolveFieldContext context)
    {
        base.Map(context);

        ResponsibleName = context.GetArgument<string>(nameof(ResponsibleName));
        StoreId = context.GetArgument<string>(nameof(StoreId));
        StartDueDate = context.GetArgument<DateTime?>(nameof(StartDueDate));
        EndDueDate = context.GetArgument<DateTime?>(nameof(EndDueDate));
        IsActive = context.GetArgument<bool?>(nameof(IsActive));
        Completed = context.GetArgument<bool?>(nameof(Completed));
    }
}
