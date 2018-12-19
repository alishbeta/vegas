using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nop.Core.Domain.Shipping;

namespace Nop.Data.Mapping.Shipping
{
    public partial class WarehousePicturesMap : NopEntityTypeConfiguration<WarehousePictures>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityTypeBuilder<WarehousePictures> builder)
        {
            builder.ToTable(nameof(WarehousePictures));
            builder.HasKey(warehouse => warehouse.Id);

            builder.Property(warehouse => warehouse.PictureId).HasMaxLength(100).IsRequired();
            builder.Property(warehouse => warehouse.WarehouseId).HasMaxLength(100).IsRequired();

            base.Configure(builder);
        }

        #endregion
    }
}
