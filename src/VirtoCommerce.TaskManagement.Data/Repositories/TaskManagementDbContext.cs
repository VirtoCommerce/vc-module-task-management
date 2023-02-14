using EntityFrameworkCore.Triggers;
using Microsoft.EntityFrameworkCore;
using VirtoCommerce.TaskManagement.Data.Models;

namespace VirtoCommerce.TaskManagement.Data.Repositories
{
    public class TaskManagementDbContext : DbContextWithTriggers
    {
        private const int _maxLength = 128;

        public TaskManagementDbContext(DbContextOptions<TaskManagementDbContext> options)
            : base(options)
        {
        }

        protected TaskManagementDbContext(DbContextOptions options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<WorkTaskEntity>().ToTable("WorkTask").HasKey(x => x.Id);
            modelBuilder.Entity<WorkTaskEntity>().Property(x => x.Id).HasMaxLength(_maxLength).ValueGeneratedOnAdd();
            modelBuilder.Entity<WorkTaskEntity>().Property(x => x.Number).ValueGeneratedOnAdd();
            modelBuilder.Entity<WorkTaskEntity>().HasIndex(x => x.Status);
            modelBuilder.Entity<WorkTaskEntity>().HasIndex(x => x.IsActive);
            modelBuilder.Entity<WorkTaskEntity>().HasIndex(x => x.Completed);
            modelBuilder.Entity<WorkTaskEntity>().HasIndex(x => x.DueDate);
            modelBuilder.Entity<WorkTaskEntity>().HasIndex(x => x.ResponsibleId);
            modelBuilder.Entity<WorkTaskEntity>().HasIndex(x => x.StoreId);
            modelBuilder.Entity<WorkTaskEntity>().HasIndex(x => x.ObjectId);
            modelBuilder.Entity<WorkTaskEntity>().HasIndex(x => x.ObjectType);
            modelBuilder.Entity<WorkTaskEntity>().HasIndex(x => x.Number);

            modelBuilder.Entity<WorkTaskAttachmentEntity>().ToTable("WorkTaskAttachment").HasKey(x => x.Id);
            modelBuilder.Entity<WorkTaskAttachmentEntity>().Property(x => x.Id).HasMaxLength(128).ValueGeneratedOnAdd();
            modelBuilder.Entity<WorkTaskAttachmentEntity>().HasOne(x => x.WorkTask).WithMany(x => x.Attachments)
                .HasForeignKey(x => x.WorkTaskId).IsRequired().OnDelete(DeleteBehavior.Cascade);
        }
    }
}
