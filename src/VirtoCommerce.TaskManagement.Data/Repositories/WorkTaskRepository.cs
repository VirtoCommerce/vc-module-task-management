using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Data.Infrastructure;
using VirtoCommerce.TaskManagement.Core.Models;
using VirtoCommerce.TaskManagement.Data.Models;

namespace VirtoCommerce.TaskManagement.Data.Repositories
{
    public class WorkTaskRepository : DbContextRepositoryBase<TaskManagementDbContext>, IWorkTaskRepository
    {
        public WorkTaskRepository(TaskManagementDbContext dbContext)
            : base(dbContext)
        {
        }

        public IQueryable<WorkTaskEntity> WorkTasks => DbContext.Set<WorkTaskEntity>();
        public IQueryable<WorkTaskAttachmentEntity> WorkTaskAttachments => DbContext.Set<WorkTaskAttachmentEntity>();

        public async Task<IEnumerable<WorkTaskEntity>> GetWorkTaskByIds(IList<string> ids, string responseGroup = null)
        {
            IList<WorkTaskEntity> result = null;

            if (ids?.Any() == true)
            {
                result = ids.Count == 1
                    ? await WorkTasks.Where(x => x.Id == ids.First()).ToListAsync()
                    : await WorkTasks.Where(x => ids.Contains(x.Id)).ToListAsync();
            }

            var workTaskResponseGroup = EnumUtility.SafeParseFlags(responseGroup, WorkTaskResponseGroup.Full);

            if (workTaskResponseGroup.HasFlag(WorkTaskResponseGroup.WithAttachments))
            {
                await WorkTaskAttachments.Where(x => ids.Contains(x.WorkTaskId)).LoadAsync();
            }

            return result ?? Array.Empty<WorkTaskEntity>();
        }
    }
}
