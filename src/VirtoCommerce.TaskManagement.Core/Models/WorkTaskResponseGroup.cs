using System;

namespace VirtoCommerce.TaskManagement.Core.Models
{
    [Flags]
    public enum WorkTaskResponseGroup
    {
        Default = 0,
        WithAttachments = 1,
        Full = WithAttachments
    }
}
