using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using VirtoCommerce.Platform.Core.Caching;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Events;
using VirtoCommerce.Platform.Data.GenericCrud;
using VirtoCommerce.TaskManagement.Core.Events;
using VirtoCommerce.TaskManagement.Core.Models;
using VirtoCommerce.TaskManagement.Core.Services;
using VirtoCommerce.TaskManagement.Data.Models;
using VirtoCommerce.TaskManagement.Data.Repositories;

namespace VirtoCommerce.TaskManagement.Data.Services
{
    public class WorkTaskService : CrudService<WorkTask, WorkTaskEntity, WorkTaskChangingEvent, WorkTaskChangedEvent>, IWorkTaskService
    {
        public WorkTaskService(Func<IWorkTaskRepository> repositoryFactory, IPlatformMemoryCache platformMemoryCache, IEventPublisher eventPublisher)
            : base(repositoryFactory, platformMemoryCache, eventPublisher)
        {
        }

        public virtual async Task<WorkTask> CancelAsync(string id, JObject result)
        {
            var workTask = await GetByIdAsync(id);
            var originalWorkTask = (WorkTask)workTask.Clone();

            workTask.Completed = false;
            workTask.IsActive = false;
            workTask.Result = result;

            await SaveChangesAsync(new[] { workTask });

            var workTaskCanceledEvent = new WorkTaskCanceledEvent(new[]
            {
                new GenericChangedEntry<WorkTask>(workTask, originalWorkTask, EntryState.Modified)
            });

            await _eventPublisher.Publish(workTaskCanceledEvent);

            return workTask;
        }

        public virtual async Task<WorkTask> CompleteAsync(string id, JObject result)
        {
            var workTask = await GetByIdAsync(id);
            var originalWorkTask = (WorkTask)workTask.Clone();

            workTask.Completed = true;
            workTask.IsActive = false;
            workTask.Result = result;

            await SaveChangesAsync(new[] { workTask });

            var workTaskCompletedEvent = new WorkTaskCompletedEvent(new[]
            {
                new GenericChangedEntry<WorkTask>(workTask, originalWorkTask, EntryState.Modified)
            });

            await _eventPublisher.Publish(workTaskCompletedEvent);

            return workTask;
        }

        public async Task<WorkTask> TimeoutAsync(string id)
        {
            var workTask = await GetByIdAsync(id);
            workTask.IsActive = false;
            await SaveChangesAsync(new[] { workTask });

            return workTask;
        }

        protected override async Task<IEnumerable<WorkTaskEntity>> LoadEntities(IRepository repository, IEnumerable<string> ids, string responseGroup)
        {
            return await ((IWorkTaskRepository)repository).GetWorkTaskByIds(ids.ToList());
        }
    }
}
