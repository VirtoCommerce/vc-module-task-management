using System.Collections.Generic;
using VirtoCommerce.Platform.Core.Events;
using VirtoCommerce.TaskManagement.Core.Models;

namespace VirtoCommerce.TaskManagement.Core.Events
{
    public class WorkTaskCanceledEvent : GenericChangedEntryEvent<WorkTask>
    {
        public WorkTaskCanceledEvent(IEnumerable<GenericChangedEntry<WorkTask>> changedEntries)
            : base(changedEntries)
        {
        }
    }
}
