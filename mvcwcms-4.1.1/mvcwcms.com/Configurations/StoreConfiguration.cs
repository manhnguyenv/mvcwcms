using MVCwCMS.Models;
using System.Data.Entity.ModelConfiguration;

namespace MVCwCMS.Configurations
{
    public class StoreConfiguration : EntityTypeConfiguration<Store>
    {
        public StoreConfiguration()
        {
            ToTable("Store");
            Property(c => c.Name).IsRequired().HasMaxLength(255);
            Property(c => c.Description).IsOptional().HasMaxLength(255);
            Property(c => c.Address).IsOptional().HasMaxLength(255);
            Property(c => c.Phone).IsOptional().HasMaxLength(255);
            Property(c => c.Owner).IsOptional();
        }
    }
}