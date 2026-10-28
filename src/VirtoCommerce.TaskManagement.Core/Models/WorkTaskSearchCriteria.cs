using System;
using System.Collections.Generic;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.TaskManagement.Core.Models
{
    public class WorkTaskSearchCriteria : SearchCriteriaBase
    {
        public IList<string> ResponsibleIds { get; set; }
        public IList<string> OrganizationIds { get; set; }

        public IList<string> StoreIds { get; set; }

        // The record a task is linked to (WorkTask.ObjectType/ObjectId). Not ObjectIds: those are the tasks' own ids.
        public string LinkedObjectType { get; set; }
        public IList<string> LinkedObjectIds { get; set; }

        public DateTime? StartDueDate { get; set; }
        public DateTime? EndDueDate { get; set; }

        public string Priority { get; set; }

        private IList<string> _priorities;
        public IList<string> Priorities
        {
            get
            {
                // Not stored, unlike ObjectTypes: a criteria searched or cloned once still follows Priority.
                return _priorities ?? (string.IsNullOrEmpty(Priority) ? null : new[] { Priority });
            }
            set
            {
                _priorities = value;
            }
        }

        public string Type { get; set; }

        public bool? OnlyAssignedToMe { get; set; }
        public bool? IsActive { get; set; }
        public bool? Completed { get; set; }

    }
}
