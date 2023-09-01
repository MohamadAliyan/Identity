using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Identity.Models;

namespace Identity.config
{

    public class UserConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {
        public virtual void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {

            builder.ToTable("Users");
            builder.Property(e => e.Family)
                .HasColumnType("nvarchar(100)")
                .IsRequired(true);
            builder.Property(e => e.Email).HasColumnType("nvarchar(100)");

        }
    }
}
