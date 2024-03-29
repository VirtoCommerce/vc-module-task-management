using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Options;
using VirtoCommerce.Platform.Core.Caching;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.GenericCrud;
using VirtoCommerce.Platform.Data.GenericCrud;
using VirtoCommerce.TaskManagement.Core.Models;
using VirtoCommerce.TaskManagement.Core.Services;
using VirtoCommerce.TaskManagement.Data.Models;
using VirtoCommerce.TaskManagement.Data.Repositories;

namespace VirtoCommerce.TaskManagement.Data.Services
{
    public class WorkTaskSearchService : SearchService<WorkTaskSearchCriteria, WorkTaskSearchResult, WorkTask, WorkTaskEntity>, IWorkTaskSearchService
    {
        public WorkTaskSearchService(Func<IWorkTaskRepository> repositoryFactory, IPlatformMemoryCache platformMemoryCache, IWorkTaskService crudService, IOptions<CrudOptions> crudOptions)
            : base(repositoryFactory, platformMemoryCache, crudService, crudOptions)
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

            if (!criteria.ResponsibleIds.IsNullOrEmpty())
            {
                query = criteria.ResponsibleIds.Count == 1
                    ? query.Where(x => x.ResponsibleId == criteria.ResponsibleIds.First())
                    : query.Where(x => criteria.ResponsibleIds.Contains(x.ResponsibleId));
            }

            if (!criteria.OrganizationIds.IsNullOrEmpty())
            {
                query = criteria.OrganizationIds.Count == 1
                    ? query.Where(x => x.OrganizationId == criteria.OrganizationIds.First())
                    : query.Where(x => criteria.OrganizationIds.Contains(x.OrganizationId));
            }

            if (criteria.Priority != null)
            {
                query = query.Where(x => x.Priority == criteria.Priority);
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

            if (!criteria.Keyword.IsNullOrEmpty())
            {
                query = query.Where(x => x.Name.Contains(criteria.Keyword) || x.Description.Contains(criteria.Keyword));
            }

            if (!criteria.Type.IsNullOrEmpty())
            {
                query = query.Where(x => x.Type.Contains(criteria.Type));
            }

            return query;
        }

        protected override IList<SortInfo> BuildSortExpression(WorkTaskSearchCriteria criteria)
        {
            var sortInfos = criteria.SortInfos;
            if (sortInfos.IsNullOrEmpty())
            {
                sortInfos = new[]
                {
                    new SortInfo
                    {
                        SortColumn = nameof(WorkTaskEntity.CreatedDate),
                        SortDirection = SortDirection.Descending
                    }
                };
            }
            return sortInfos;
        }
    }
}
