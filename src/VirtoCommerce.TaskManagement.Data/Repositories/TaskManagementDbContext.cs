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
            modelBuilder.Entity<WorkTaskEntity>().HasIndex(x => x.IsActive);
            modelBuilder.Entity<WorkTaskEntity>().HasIndex(x => x.Completed);
            modelBuilder.Entity<WorkTaskEntity>().HasIndex(x => x.DueDate);
            modelBuilder.Entity<WorkTaskEntity>().HasIndex(x => x.ResponsibleName);
            modelBuilder.Entity<WorkTaskEntity>().HasIndex(x => x.StoreId);
            modelBuilder.Entity<WorkTaskEntity>().HasIndex(x => x.ObjectId);
            modelBuilder.Entity<WorkTaskEntity>().HasIndex(x => x.ObjectType);
        }
    }
}
