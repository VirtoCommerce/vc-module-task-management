using EntityFrameworkCore.Triggers;
using Microsoft.EntityFrameworkCore;

namespace VirtoCommerce.TaskManagement.Data.Repositories
{
    public class TaskManagementDbContext : DbContextWithTriggers
    {
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

            //modelBuilder.Entity<TaskManagementEntity>().ToTable("TaskManagement").HasKey(x => x.Id);
            //modelBuilder.Entity<TaskManagementEntity>().Property(x => x.Id).HasMaxLength(128).ValueGeneratedOnAdd();
        }
    }
}
