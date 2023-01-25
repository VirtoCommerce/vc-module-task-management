using System.Threading;
using System.Threading.Tasks;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;
using VirtoCommerce.TaskManagement.Core.Models;
using VirtoCommerce.TaskManagement.Core.Services;

namespace VirtoCommerce.TaskManagement.ExperienceApi.Queries;

public class QuotesQueryHandler : IQueryHandler<WorkTasksQuery, WorkTaskSearchResult>
{
    private readonly IWorkTaskSearchService _workTaskSearchService;

    public QuotesQueryHandler(IWorkTaskSearchService workTaskSearchService)
    {
        _workTaskSearchService = workTaskSearchService;
    }

    public virtual async Task<WorkTaskSearchResult> Handle(WorkTasksQuery request, CancellationToken cancellationToken)
    {
        var criteria = GetSearchCriteria(request);
        var result = await _workTaskSearchService.SearchAsync(criteria);

        return result;
    }


    protected virtual WorkTaskSearchCriteria GetSearchCriteria(WorkTasksQuery request)
    {
        var criteria = request.GetSearchCriteria<WorkTaskSearchCriteria>();

        if (!string.IsNullOrEmpty(request.ResponsibleId))
        {
            criteria.ResponsibleIds = new[] { request.ResponsibleId };
        }

        if (!string.IsNullOrEmpty(request.StoreId))
        {
            criteria.StoreIds = new[] { request.StoreId };
        }

        criteria.StartDueDate = request.StartDueDate;
        criteria.EndDueDate = request.EndDueDate;
        criteria.IsActive = request.IsActive;
        criteria.Completed = request.Completed;

        return criteria;
    }
}
