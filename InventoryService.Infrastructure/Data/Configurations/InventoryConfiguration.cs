using InventoryService.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InventoryService.Infrastructure.Data.Configurations;

/// <summary>
/// Configures the Inventory entity type.
/// </summary>
/// <param name="builder">The builder used to configure the Inventory entity.</param>
public class InventoryConfiguration : IEntityTypeConfiguration<Inventory>
{
    public void Configure(EntityTypeBuilder<Inventory> builder)
    {
        builder.HasKey(i => i.Id);

        builder.Property(b => b.ProductId)
            .IsRequired();

        builder.Property(b => b.ProductName)
            .IsRequired();

        builder.Property(b => b.Quantity)
            .IsRequired();
    }
}
