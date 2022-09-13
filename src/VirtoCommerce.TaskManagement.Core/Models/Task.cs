using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.TaskManagement.Core.Models
{
    public class Task : AuditableEntity, ICloneable
    {
        public string StoreId { get; set; }

        public string Type { get; set; }

        public string Description { get; set; }

        public TaskPriority Priority { get; set; }

        public string Responsible { get; set; }
        public string ResponsibleName { get; set; }

        public DateTime? DueDate { get; set; }

        public bool IsCompleted { get; set; }

        public JObject Parameters { get; set; }

        public JObject Result { get; set; }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
