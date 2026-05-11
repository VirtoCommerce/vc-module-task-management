using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VirtoCommerce.TaskManagement.Data.Models;

namespace VirtoCommerce.TaskManagement.Data.MySql;

public class WorkTaskEntityConfiguration : IEntityTypeConfiguration<WorkTaskEntity>
{
    public void Configure(EntityTypeBuilder<WorkTaskEntity> builder)
    {
        // MySQL requires AUTO_INCREMENT columns to be part of a key AT CREATE TABLE time.
        // A separate unique index after CREATE TABLE is rejected. Declaring Number as an
        // alternate key makes EF emit an inline UNIQUE constraint inside the CREATE TABLE.
        builder.HasAlternateKey(x => x.Number);
    }
}
