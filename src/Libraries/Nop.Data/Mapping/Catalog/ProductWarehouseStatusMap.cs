using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Nop.Core.Domain.Catalog;

namespace Nop.Data.Mapping.Catalog
{
    public class ProductWarehouseStatusMap : NopEntityTypeConfiguration<ProductWarehouseStatus>
    {
        public override void Configure(EntityTypeBuilder<ProductWarehouseStatus> builder)
        {
            builder.ToTable(nameof(ProductWarehouseStatus));
            builder.HasKey(s => s.Id);
            builder.Property(s => s.ProductId).IsRequired();
            builder.Property(s => s.WarehouseId).IsRequired();
            base.Configure(builder);
        }
    }
}
