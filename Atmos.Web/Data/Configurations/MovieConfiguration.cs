using Atmos.Web.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Atmos.Web.Data.Configurations
{
    public class MovieConfiguration : IEntityTypeConfiguration<Movie>
    {
        public void Configure(EntityTypeBuilder<Movie> builder)
        {
            builder.ToTable("Movies");
            builder.Property(m => m.Id)
                .HasColumnName("Id");
            builder.HasMany(m => m.Subtitles)
                .WithOne(s => s.Movie);
        }
    }
}