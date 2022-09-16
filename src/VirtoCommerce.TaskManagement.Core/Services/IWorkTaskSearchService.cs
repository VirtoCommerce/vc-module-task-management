using VirtoCommerce.Platform.Core.GenericCrud;
using VirtoCommerce.TaskManagement.Core.Models;

namespace VirtoCommerce.TaskManagement.Core.Services
{
    public interface IWorkTaskSearchService : ISearchService<WorkTaskSearchCriteria, WorkTaskSearchResult, WorkTask>
    {
    }
}
