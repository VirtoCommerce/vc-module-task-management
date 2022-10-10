using System;
using System.Linq;
using VirtoCommerce.Platform.Core.Caching;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Data.GenericCrud;
using VirtoCommerce.TaskManagement.Core.Models;
using VirtoCommerce.TaskManagement.Core.Services;
using VirtoCommerce.TaskManagement.Data.Models;
using VirtoCommerce.TaskManagement.Data.Repositories;

namespace VirtoCommerce.TaskManagement.Data.Services
{
    public class WorkTaskSearchService : SearchService<WorkTaskSearchCriteria, WorkTaskSearchResult, WorkTask, WorkTaskEntity>, IWorkTaskSearchService
    {
        public WorkTaskSearchService(Func<IWorkTaskRepository> repositoryFactory, IPlatformMemoryCache platformMemoryCache, IWorkTaskService crudService)
            : base(repositoryFactory, platformMemoryCache, crudService)
        {
        }

        protected override IQueryable<WorkTaskEntity> BuildQuery(IRepository repository, WorkTaskSearchCriteria criteria)
        {
            var query = ((IWorkTaskRepository)repository).WorkTasks;

            if (!criteria.ObjectIds.IsNullOrEmpty())
            {
                query = criteria.ObjectIds.Count == 1
                    ? query.Where(x => x.Id == criteria.ObjectIds.First())
                    : query.Where(x => criteria.ObjectIds.Contains(x.Id));
            }

            if (!criteria.StoreIds.IsNullOrEmpty())
            {
                query = criteria.StoreIds.Count == 1
                    ? query.Where(x => x.StoreId == criteria.StoreIds.First())
                    : query.Where(x => criteria.StoreIds.Contains(x.StoreId));
            }

            if (!criteria.ResponsibleNames.IsNullOrEmpty())
            {
                query = criteria.ResponsibleNames.Count == 1
                    ? query.Where(x => x.ResponsibleName == criteria.ResponsibleNames.First())
                    : query.Where(x => criteria.ResponsibleNames.Contains(x.ResponsibleName));
            }

            if (criteria.IsActive != null)
            {
                query = query.Where(x => x.IsActive == criteria.IsActive);
            }

            if (criteria.Completed != null)
            {
                query = query.Where(x => x.Completed == criteria.Completed);
            }

            if (criteria.StartDueDate != null)
            {
                query = query.Where(x => x.DueDate >= criteria.StartDueDate);
            }

            if (criteria.EndDueDate != null)
            {
                query = query.Where(x => x.DueDate <= criteria.EndDueDate);
            }

            return query;
        }
    }
}
