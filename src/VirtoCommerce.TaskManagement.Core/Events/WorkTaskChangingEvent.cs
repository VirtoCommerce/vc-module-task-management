using System.Collections.Generic;
using VirtoCommerce.Platform.Core.Events;
using VirtoCommerce.TaskManagement.Core.Models;

namespace VirtoCommerce.TaskManagement.Core.Events
{
    public class WorkTaskChangingEvent : GenericChangedEntryEvent<WorkTask>
    {
        public WorkTaskChangingEvent(IEnumerable<GenericChangedEntry<WorkTask>> changedEntries)
            : base(changedEntries)
        {
        }
    }
}
