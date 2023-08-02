using System;
using System.Collections.Generic;
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
        private readonly IEventPublisher _eventPublisher;

        public WorkTaskService(Func<IWorkTaskRepository> repositoryFactory, IPlatformMemoryCache platformMemoryCache, IEventPublisher eventPublisher)
            : base(repositoryFactory, platformMemoryCache, eventPublisher)
        {
            _eventPublisher = eventPublisher;
        }

        public virtual async Task<WorkTask> FinishAsync(string id, bool completed, JObject result)
        {
            var workTask = await this.GetByIdAsync(id);
            var originalWorkTask = (WorkTask)workTask.Clone();

            workTask.Completed = completed;
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

        public async Task<WorkTask> TimeoutAsync(string id)
        {
            var workTask = await this.GetByIdAsync(id);
            workTask.IsActive = false;
            await SaveChangesAsync(new[] { workTask });

            return workTask;
        }

        protected override Task<IList<WorkTaskEntity>> LoadEntities(IRepository repository, IList<string> ids, string responseGroup)
        {
            return ((IWorkTaskRepository)repository).GetWorkTaskByIds(ids, responseGroup);
        }
    }
}
