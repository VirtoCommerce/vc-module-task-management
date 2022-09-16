using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using VirtoCommerce.Platform.Core.GenericCrud;
using VirtoCommerce.TaskManagement.Core.Models;

namespace VirtoCommerce.TaskManagement.Core.Services
{
    public interface IWorkTaskService : ICrudService<WorkTask>
    {
        Task<WorkTask> CompleteAsync(string id, JObject result);
        Task<WorkTask> CancelAsync(string id, JObject result);
    }
}
