using System.Collections.Generic;
using VirtoCommerce.Platform.Core.Events;
using VirtoCommerce.TaskManagement.Core.Models;

namespace VirtoCommerce.TaskManagement.Core.Events
{
    public class WorkTaskChangedEvent : GenericChangedEntryEvent<WorkTask>
    {
        public WorkTaskChangedEvent(IEnumerable<GenericChangedEntry<WorkTask>> changedEntries)
            : base(changedEntries)
        {
        }
    }
}
