using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using VirtoCommerce.Platform.Core.GenericCrud;
using VirtoCommerce.TaskManagement.Core.Models;

namespace VirtoCommerce.TaskManagement.Core.Services
{
    public interface IWorkTaskService : ICrudService<WorkTask>
    {
        Task<WorkTask> ApproveAsync(string id, JObject result);
        Task<WorkTask> DeclineAsync(string id, JObject result);
        Task<WorkTask> TimeoutAsync(string id);
    }
}
