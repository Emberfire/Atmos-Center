using Atmos.Web.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Atmos.Web.Data.Configurations
{
    public class SubtitleConfiguration : IEntityTypeConfiguration<Subtitle>
    {
        public void Configure(EntityTypeBuilder<Subtitle> builder)
        {
            builder.ToTable("Subtitles");
            builder.Property(s => s.Id)
                .HasColumnName("Id");
            builder.HasOne(s => s.Movie)
                .WithMany(m => m.Subtitles);
        }
    }
}