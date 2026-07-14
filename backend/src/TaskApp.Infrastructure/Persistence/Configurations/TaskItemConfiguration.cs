using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskApp.Domain.Entities;

namespace TaskApp.Infrastructure.Persistence.Configurations;

/// <summary>
/// Fluent API mapping for <see cref="TaskItem"/>. Keeps the domain entity free of
/// persistence attributes, per the dependency rule (Domain has no external dependencies).
/// </summary>
public class TaskItemConfiguration : IEntityTypeConfiguration<TaskItem>
{
    /// <summary>Configures the table mapping, constraints, and conversions for <see cref="TaskItem"/>.</summary>
    /// <param name="builder">Builder used to configure the <see cref="TaskItem"/> entity type.</param>
    public void Configure(EntityTypeBuilder<TaskItem> builder)
    {
        builder.ToTable("Tasks");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Title)
            .IsRequired()
            .HasMaxLength(200);

        // Stored as text (e.g. "Medium") instead of the EF Core default int, so the
        // raw SQLite file is readable without decoding the enum by hand.
        builder.Property(t => t.Priority)
            .HasConversion<string>()
            .HasMaxLength(10)
            .IsRequired();

        builder.Property(t => t.CreatedAt)
            .IsRequired();
    }
}
