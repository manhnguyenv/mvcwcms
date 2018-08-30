using MVCwCMS.Models;
using System.Data.Entity.ModelConfiguration;

namespace MVCwCMS.Configurations
{
    public class TeamConfiguration : EntityTypeConfiguration<Team>
    {
        public TeamConfiguration()
        {
            ToTable("Team");
            Property(c => c.Name).IsRequired().HasMaxLength(255);
            Property(c => c.Description).IsOptional().HasMaxLength(255);
            Property(c => c.Slogan).IsOptional().HasMaxLength(255);
        }
    }
}