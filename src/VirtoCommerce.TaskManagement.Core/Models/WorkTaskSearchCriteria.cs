using System;
using System.Collections.Generic;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.TaskManagement.Core.Models
{
    public class WorkTaskSearchCriteria : SearchCriteriaBase
    {
        public IList<string> ResponsibleIds { get; set; }

        public IList<string> StoreIds { get; set; }

        public DateTime? StartDueDate { get; set; }
        public DateTime? EndDueDate { get; set; }

        public string Priority { get; set; }
        public bool? IsActive { get; set; }
        public bool? Completed { get; set; }
    }
}
