using System;
using System.Collections.Generic;
using GraphQL;
using GraphQL.Types;
using VirtoCommerce.Xapi.Core.BaseQueries;
using VirtoCommerce.TaskManagement.Core.Models;

namespace VirtoCommerce.TaskManagement.ExperienceApi.Queries;

public class WorkTasksQuery : SearchQuery<WorkTaskSearchResult>
{
    public string ResponsibleId { get; set; }
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

        yield return Argument<StringGraphType>(nameof(ResponsibleId));
        yield return Argument<StringGraphType>(nameof(StoreId));
        yield return Argument<DateTimeGraphType>(nameof(StartDueDate));
        yield return Argument<DateTimeGraphType>(nameof(EndDueDate));
        yield return Argument<BooleanGraphType>(nameof(IsActive));
        yield return Argument<BooleanGraphType>(nameof(Completed));
    }

    public override void Map(IResolveFieldContext context)
    {
        base.Map(context);

        ResponsibleId = context.GetArgument<string>(nameof(ResponsibleId));
        StoreId = context.GetArgument<string>(nameof(StoreId));
        StartDueDate = context.GetArgument<DateTime?>(nameof(StartDueDate));
        EndDueDate = context.GetArgument<DateTime?>(nameof(EndDueDate));
        IsActive = context.GetArgument<bool?>(nameof(IsActive));
        Completed = context.GetArgument<bool?>(nameof(Completed));
    }
}
