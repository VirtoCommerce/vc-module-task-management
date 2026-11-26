using VirtoCommerce.CustomerModule.Core.Model;

namespace VirtoCommerce.TaskManagement.Core.Models
{
    /// <summary>
    /// A member that can be responsible for a task, with only the details the task management app shows
    /// </summary>
    public class TaskMember
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string MemberType { get; set; }

        public virtual TaskMember FromModel(Member member)
        {
            Id = member.Id;
            Name = member.Name;
            MemberType = member.MemberType;

            return this;
        }
    }
}
