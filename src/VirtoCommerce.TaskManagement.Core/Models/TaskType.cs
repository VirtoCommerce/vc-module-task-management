using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.TaskManagement.Core.Models
{
    public class TaskType : AuditableEntity, ICloneable
    {
        public string Code { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }

}
