using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.TaskManagement.Core.Models
{
    public class WorkTask : AuditableEntity, ICloneable
    {
        public long Number { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public string Group { get; set; }
        public string Type { get; set; }
        public TaskPriority Priority { get; set; }

        public string ResponsibleId { get; set; }
        public string ResponsibleName { get; set; }

        public DateTime? DueDate { get; set; }

        public string Status { get; set; }
        public bool IsActive { get; set; }
        public bool? Completed { get; set; }

        public string StoreId { get; set; }
        public string WorkflowId { get; set; }
        public JObject Parameters { get; set; }
        public JObject Result { get; set; }

        public string ObjectId { get; set; }
        public string ObjectType { get; set; }

        public ICollection<WorkTaskAttachment> Attachments { get; set; } = new List<WorkTaskAttachment>();

        public object Clone()
        {
            var clone = MemberwiseClone() as WorkTask;
            clone.Attachments = Attachments?.Select(x => x.Clone()).OfType<WorkTaskAttachment>().ToList();
            return clone;
        }
    }
}
