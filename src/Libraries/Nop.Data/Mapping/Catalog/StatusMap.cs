using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nop.Core.Domain.Catalog;

namespace Nop.Data.Mapping.Catalog
{
    public class StatusMap : NopEntityTypeConfiguration<Status>
    {
        public override void Configure(EntityTypeBuilder<Status> builder)
        {
            builder.ToTable(nameof(Status));
            builder.HasKey(s => s.Id);
            builder.Property(s => s.Name).IsRequired().HasMaxLength(50);
            base.Configure(builder);
        }
    }
}
