using System;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Domain;
using VirtoCommerce.TaskManagement.Core.Models;

namespace VirtoCommerce.TaskManagement.Data.Models
{
    public class WorkTaskEntity : AuditableEntity, IDataEntity<WorkTaskEntity, WorkTask>
    {
        [StringLength(128)]
        public string StoreId { get; set; }

        [StringLength(128)]
        public string WorkflowId { get; set; }

        [StringLength(64)]
        public string Type { get; set; }

        public string Description { get; set; }

        [StringLength(64)]
        public string Priority { get; set; }

        [StringLength(256)]
        public string ResponsibleName { get; set; }

        public DateTime? DueDate { get; set; }

        public bool IsActive { get; set; }
        public bool? Completed { get; set; }

        public string Parameters { get; set; }

        public string Result { get; set; }

        [StringLength(128)]
        public string ObjectId { get; set; }
        [StringLength(256)]
        public string ObjectType { get; set; }

        public WorkTaskEntity FromModel(WorkTask model, PrimaryKeyResolvingMap pkMap)
        {
            pkMap.AddPair(model, this);

            Id = model.Id;

            CreatedDate = model.CreatedDate;
            ModifiedDate = model.ModifiedDate;
            CreatedBy = model.CreatedBy;
            ModifiedBy = model.ModifiedBy;

            StoreId = model.StoreId;
            WorkflowId = model.WorkflowId;
            Type = model.Type;
            Description = model.Description;
            Priority = model.Priority.ToString();
            ResponsibleName = model.ResponsibleName;
            DueDate = model.DueDate;
            IsActive = model.IsActive;
            Completed = model.Completed;
            Parameters = JsonConvert.SerializeObject(model.Parameters);
            Result = JsonConvert.SerializeObject(model.Result);
            ObjectId = model.ObjectId;
            ObjectType = model.ObjectType;

            return this;
        }

        public WorkTask ToModel(WorkTask model)
        {
            model.Id = Id;

            model.CreatedDate = CreatedDate;
            model.ModifiedDate = ModifiedDate;
            model.CreatedBy = CreatedBy;
            model.ModifiedBy = ModifiedBy;

            model.StoreId = StoreId;
            model.WorkflowId = WorkflowId;
            model.Type = Type;
            model.Description = Description;
            model.Priority = EnumUtility.SafeParse(Priority, TaskPriority.Low);
            model.ResponsibleName = ResponsibleName;
            model.DueDate = DueDate;
            model.IsActive = IsActive;
            model.Completed = Completed;
            model.Parameters = JsonConvert.DeserializeObject<JObject>(Parameters);
            model.Result = JsonConvert.DeserializeObject<JObject>(Result);
            model.ObjectId = ObjectId;
            model.ObjectType = ObjectType;

            return model;
        }

        public void Patch(WorkTaskEntity target)
        {
            target.StoreId = StoreId;
            target.WorkflowId = WorkflowId;
            target.Type = Type;
            target.Description = Description;
            target.Priority = Priority;
            target.ResponsibleName = ResponsibleName;
            target.DueDate = DueDate;
            target.IsActive = IsActive;
            target.Completed = Completed;
            target.Parameters = Parameters;
            target.Result = Result;
            target.ObjectId = ObjectId;
            target.ObjectType = ObjectType;
        }
    }
}
