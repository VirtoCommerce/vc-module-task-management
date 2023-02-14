using System;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.TaskManagement.Core.Models
{
    public class WorkTaskAttachment : AuditableEntity, ICloneable
    {
        public string Name { get; set; }
        public string Url { get; set; }

        public virtual object Clone()
        {
            return MemberwiseClone();
        }
    }
}
