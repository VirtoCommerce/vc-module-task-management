using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.TaskManagement.Data.Models;

namespace VirtoCommerce.TaskManagement.Data.Repositories
{
    public interface IWorkTaskRepository : IRepository
    {
        IQueryable<WorkTaskEntity> WorkTasks { get; }

        Task<IEnumerable<WorkTaskEntity>> GetWorkTaskByIds(IList<string> ids);
    }
}
