using InventoryService.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InventoryService.Infrastructure.Data.Configurations;

public class InventoryHistoryConfiguration : IEntityTypeConfiguration<InventoryHistory>
{
    public void Configure(EntityTypeBuilder<InventoryHistory> builder)
    {
        builder.HasKey(ih => ih.Id);

        builder.Property(b => b.ProductId)
            .IsRequired();

        builder.Property(b => b.InventoryId)
            .IsRequired();

        builder.Property(b => b.OldQuantity)
            .IsRequired();

        builder.Property(b => b.NewQuantity)
            .IsRequired();

        builder.Property(b => b.Timestamp)
            .IsRequired();

        builder.HasOne(b => b.Inventory)
            .WithMany()
            .HasForeignKey(ih => ih.InventoryId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
