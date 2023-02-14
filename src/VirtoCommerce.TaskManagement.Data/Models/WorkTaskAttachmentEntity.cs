using System.ComponentModel.DataAnnotations;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Domain;
using VirtoCommerce.TaskManagement.Core.Models;

namespace VirtoCommerce.TaskManagement.Data.Models
{
    public class WorkTaskAttachmentEntity : AuditableEntity, IDataEntity<WorkTaskAttachmentEntity, WorkTaskAttachment>
    {
        [StringLength(1024)]
        public string Url { get; set; }

        [StringLength(1024)]
        public string Name { get; set; }

        #region Navigation Properties

        public virtual WorkTaskEntity WorkTask { get; set; }
        public string WorkTaskId { get; set; }

        #endregion


        public WorkTaskAttachmentEntity FromModel(WorkTaskAttachment model, PrimaryKeyResolvingMap pkMap)
        {
            pkMap.AddPair(model, this);

            Id = model.Id;
            CreatedBy = model.CreatedBy;
            CreatedDate = model.CreatedDate;
            ModifiedBy = model.ModifiedBy;
            ModifiedDate = model.ModifiedDate;

            Name = model.Name;
            Url = model.Url;

            return this;
        }

        public WorkTaskAttachment ToModel(WorkTaskAttachment model)
        {
            model.Id = Id;
            model.CreatedBy = CreatedBy;
            model.CreatedDate = CreatedDate;
            model.ModifiedBy = ModifiedBy;
            model.ModifiedDate = ModifiedDate;

            model.Url = Url;
            model.Name = Name;

            return model;
        }

        public void Patch(WorkTaskAttachmentEntity target)
        {
            target.Url = Url;
            target.Name = Name;
        }
    }
}
