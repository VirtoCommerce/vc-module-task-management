using System.Collections.Generic;
using VirtoCommerce.Platform.Core.Events;
using VirtoCommerce.TaskManagement.Core.Models;

namespace VirtoCommerce.TaskManagement.Core.Events
{
    public class WorkTaskCompletedEvent : GenericChangedEntryEvent<WorkTask>
    {
        public WorkTaskCompletedEvent(IEnumerable<GenericChangedEntry<WorkTask>> changedEntries)
            : base(changedEntries)
        {
        }
    }
}
