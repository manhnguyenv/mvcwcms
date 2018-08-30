using MVCwCMS.Models;
using System.Data.Entity.ModelConfiguration;

namespace MVCwCMS.Configurations
{
    public class CustomerConfiguration : EntityTypeConfiguration<Customer>
    {
        public CustomerConfiguration()
        {
            ToTable("Customer");
            Property(c => c.Name).IsRequired().HasMaxLength(255);
            Property(c => c.UserName).IsRequired().HasMaxLength(255);
            Property(c => c.Description).IsOptional().HasMaxLength(255);
            Property(c => c.Phone).IsOptional().HasMaxLength(255);
            Property(c => c.Address).IsOptional().HasMaxLength(255);
            Property(c => c.Email).IsOptional().HasMaxLength(255);
            Property(c => c.Facebook).IsOptional().HasMaxLength(255);
            Property(c => c.Zalo).IsOptional().HasMaxLength(255);
            Property(c => c.CustomerGroupId).IsOptional();
        }
    }
}