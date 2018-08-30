using MVCwCMS.Models;
using System.Data.Entity.ModelConfiguration;

namespace MVCwCMS.Configurations
{
    public class GadgetConfiguration : EntityTypeConfiguration<Gadget>
    {
        public GadgetConfiguration()
        {
            ToTable("Gadget");
            Property(g => g.Name).IsRequired().HasMaxLength(255);
            Property(g => g.Price).IsRequired().HasPrecision(8, 2);
            Property(g => g.CategoryID).IsRequired();
        }
    }
}