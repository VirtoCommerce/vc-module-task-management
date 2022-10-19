using System;
using Newtonsoft.Json.Linq;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.TaskManagement.Core.Models
{
    public class WorkTask : AuditableEntity, ICloneable
    {
        public string StoreId { get; set; }

        public string WorkflowId { get; set; }

        public string Type { get; set; }

        public string Description { get; set; }

        public TaskPriority Priority { get; set; }

        public string ResponsibleName { get; set; }

        public DateTime? DueDate { get; set; }

        public bool IsActive { get; set; }
        public bool? Completed { get; set; }

        public JObject Parameters { get; set; }

        public JObject Result { get; set; }

        public string ObjectId { get; set; }
        public string ObjectType { get; set; }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
