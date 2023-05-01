using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Domain;
using VirtoCommerce.TaskManagement.Core.Models;

namespace VirtoCommerce.TaskManagement.Data.Models
{
    public class WorkTaskEntity : AuditableEntity, IDataEntity<WorkTaskEntity, WorkTask>
    {
        public long Number { get; set; }
        [StringLength(256)]
        public string Name { get; set; }
        public string Description { get; set; }

        [StringLength(128)]
        public string Group { get; set; }
        [StringLength(128)]
        public string Type { get; set; }
        [StringLength(64)]
        public string Priority { get; set; }

        [StringLength(128)]
        public string ResponsibleId { get; set; }
        [StringLength(256)]
        public string ResponsibleName { get; set; }
        [StringLength(128)]
        public string OrganizationId { get; set; }

        public DateTime? DueDate { get; set; }

        [StringLength(64)]
        public string Status { get; set; }
        public bool IsActive { get; set; }
        public bool? Completed { get; set; }

        [StringLength(128)]
        public string StoreId { get; set; }
        [StringLength(128)]
        public string WorkflowId { get; set; }
        public string Parameters { get; set; }
        public string Result { get; set; }

        [StringLength(128)]
        public string ObjectId { get; set; }
        [StringLength(256)]
        public string ObjectType { get; set; }

        public ObservableCollection<WorkTaskAttachmentEntity> Attachments { get; set; } =
            new NullCollection<WorkTaskAttachmentEntity>();

        public WorkTaskEntity FromModel(WorkTask model, PrimaryKeyResolvingMap pkMap)
        {
            pkMap.AddPair(model, this);

            Id = model.Id;

            CreatedDate = model.CreatedDate;
            ModifiedDate = model.ModifiedDate;
            CreatedBy = model.CreatedBy;
            ModifiedBy = model.ModifiedBy;

            Number = model.Number;
            Name = model.Name;
            Description = model.Description;

            Group = model.Group;
            Type = model.Type;
            Priority = model.Priority.ToString();

            ResponsibleId = model.ResponsibleId;
            ResponsibleName = model.ResponsibleName;
            OrganizationId = model.OrganizationId;

            DueDate = model.DueDate;

            Status = model.Status;
            IsActive = model.IsActive;
            Completed = model.Completed;

            StoreId = model.StoreId;
            WorkflowId = model.WorkflowId;
            Parameters = JsonConvert.SerializeObject(model.Parameters);
            Result = JsonConvert.SerializeObject(model.Result);

            ObjectId = model.ObjectId;
            ObjectType = model.ObjectType;

            if (model.Attachments != null)
            {
                Attachments = new ObservableCollection<WorkTaskAttachmentEntity>(model.Attachments.Select(x => AbstractTypeFactory<WorkTaskAttachmentEntity>.TryCreateInstance().FromModel(x, pkMap)));
            }

            return this;
        }

        public WorkTask ToModel(WorkTask model)
        {
            model.Id = Id;

            model.CreatedDate = CreatedDate;
            model.ModifiedDate = ModifiedDate;
            model.CreatedBy = CreatedBy;
            model.ModifiedBy = ModifiedBy;

            model.Number = Number;
            model.Name = Name;
            model.Description = Description;

            model.Group = Group;
            model.Type = Type;
            model.Priority = EnumUtility.SafeParse(Priority, TaskPriority.Low);

            model.ResponsibleId = ResponsibleId;
            model.ResponsibleName = ResponsibleName;
            model.OrganizationId = OrganizationId;

            model.DueDate = DueDate;

            model.Status = Status;
            model.IsActive = IsActive;
            model.Completed = Completed;

            model.StoreId = StoreId;
            model.WorkflowId = WorkflowId;
            model.Parameters = JsonConvert.DeserializeObject<JObject>(Parameters);
            model.Result = JsonConvert.DeserializeObject<JObject>(Result);

            model.ObjectId = ObjectId;
            model.ObjectType = ObjectType;

            model.Attachments = Attachments.Select(x => x.ToModel(AbstractTypeFactory<WorkTaskAttachment>.TryCreateInstance())).ToList();

            return model;
        }

        public void Patch(WorkTaskEntity target)
        {
            target.Name = Name;
            target.Description = Description;

            target.Group = Group;
            target.Type = Type;
            target.Priority = Priority;

            target.ResponsibleId = ResponsibleId;
            target.ResponsibleName = ResponsibleName;
            target.OrganizationId = OrganizationId;

            target.DueDate = DueDate;

            target.Status = Status;
            target.IsActive = IsActive;
            target.Completed = Completed;

            target.StoreId = StoreId;
            target.WorkflowId = WorkflowId;
            target.Parameters = Parameters;
            target.Result = Result;

            target.ObjectId = ObjectId;
            target.ObjectType = ObjectType;

            if (!Attachments.IsNullCollection())
            {
                Attachments.Patch(target.Attachments, (sourceAttachment, targetAttachment) => { sourceAttachment.Patch(targetAttachment); });
            }
        }
    }
}
