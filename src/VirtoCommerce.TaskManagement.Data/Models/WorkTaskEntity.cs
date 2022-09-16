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

        [StringLength(64)]
        public string Type { get; set; }

        public string Description { get; set; }

        [StringLength(64)]
        public string Priority { get; set; }

        [StringLength(128)]
        public string ResponsibleId { get; set; }

        [StringLength(256)]
        public string ResponsibleName { get; set; }

        public DateTime? DueDate { get; set; }

        public bool IsActive { get; set; }
        public bool? Completed { get; set; }

        public string Parameters { get; set; }

        public string Result { get; set; }

        public WorkTaskEntity FromModel(WorkTask model, PrimaryKeyResolvingMap pkMap)
        {
            pkMap.AddPair(model, this);

            Id = model.Id;

            CreatedDate = model.CreatedDate;
            ModifiedDate = model.ModifiedDate;
            CreatedBy = model.CreatedBy;
            ModifiedBy = model.ModifiedBy;

            StoreId = model.StoreId;
            Type = model.Type;
            Description = model.Description;
            Priority = model.Priority.ToString();
            ResponsibleId = model.ResponsibleId;
            ResponsibleName = model.ResponsibleName;
            DueDate = model.DueDate;
            IsActive = model.IsActive;
            Completed = model.Completed;
            Parameters = JsonConvert.SerializeObject(model.Parameters);
            Result = JsonConvert.SerializeObject(model.Result);

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
            model.Type = Type;
            model.Description = Description;
            model.Priority = EnumUtility.SafeParse(Priority, TaskPriority.Low);
            model.ResponsibleId = ResponsibleId;
            model.ResponsibleName = ResponsibleName;
            model.DueDate = DueDate;
            model.IsActive = IsActive;
            model.Completed = Completed;
            model.Parameters = JsonConvert.DeserializeObject<JObject>(Parameters);
            model.Result = JsonConvert.DeserializeObject<JObject>(Result);

            return model;
        }

        public void Patch(WorkTaskEntity target)
        {
            target.StoreId = StoreId;
            target.Type = Type;
            target.Description = Description;
            target.Priority = Priority;
            target.ResponsibleId = ResponsibleId;
            target.ResponsibleName = ResponsibleName;
            target.DueDate = DueDate;
            target.IsActive = IsActive;
            target.Completed = Completed;
            target.Parameters = Parameters;
            target.Result = Result;
        }
    }
}
