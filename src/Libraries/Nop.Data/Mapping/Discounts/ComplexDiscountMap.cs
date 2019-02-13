using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nop.Core.Domain.Discounts;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Data.Mapping.Discounts
{
    /// <summary>
    /// Represents a discount mapping configuration
    /// </summary>
    public partial class ComplexDiscountMap : NopEntityTypeConfiguration<ComplexDiscount>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityTypeBuilder<ComplexDiscount> builder)
        {
            builder.ToTable(nameof(ComplexDiscount));
            builder.HasKey(discount => discount.Id);

            builder.Property(discount => discount.Name).HasMaxLength(200).IsRequired();
            builder.Property(discount => discount.DiscountPercent).HasColumnType("decimal(18, 4)").IsRequired();
            builder.Property(discount => discount.ComCollection).HasMaxLength(200);
            builder.Property(discount => discount.ComModel).HasMaxLength(200);
            builder.Property(discount => discount.ComType).HasMaxLength(200);
            builder.Property(discount => discount.InCollection).HasMaxLength(200);
            builder.Property(discount => discount.InModel).HasMaxLength(200);
            builder.Property(discount => discount.InType).HasMaxLength(200);
            builder.Property(discount => discount.GroupName).HasMaxLength(200);
            builder.Property(discount => discount.InManufacturerId).HasColumnType("int");
            builder.Property(discount => discount.ComManufacturerId).HasColumnType("int");

            base.Configure(builder);
        }

        #endregion
    }
}
