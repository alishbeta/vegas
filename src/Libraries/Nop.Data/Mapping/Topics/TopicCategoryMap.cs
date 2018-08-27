using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nop.Core.Domain.Topics;

namespace Nop.Data.Mapping.Topics
{
    public partial class TopicCategoryMap : NopEntityTypeConfiguration<TopicCategory>
    {
        public override void Configure(EntityTypeBuilder<TopicCategory> builder)
        {
            builder.ToTable(nameof(TopicCategory));
            builder.HasKey(topicCategory => topicCategory.Id);

            base.Configure(builder);
        }
    }
}
