using MVCwCMS.Models;
using System.Data.Entity.ModelConfiguration;

namespace MVCwCMS.Configurations
{
    public class CustomerGroupConfiguration : EntityTypeConfiguration<CustomerGroup>
    {
        public CustomerGroupConfiguration()
        {
            ToTable("CustomerGroup");
            Property(c => c.Name).IsRequired().HasMaxLength(255);
            Property(c => c.UserName).IsRequired().HasMaxLength(255);
        }
    }

    public class DiscountConfiguration : EntityTypeConfiguration<Discount>
    {
        public DiscountConfiguration()
        {
            ToTable("Discount");
            Property(c => c.Level).IsRequired().HasMaxLength(255);
            Property(c => c.CK).IsOptional();
            Property(c => c.Sale).IsOptional();
            Property(c => c.Capital).IsOptional();
            Property(c => c.Profit).IsOptional();
            Property(c => c.Bonus).IsOptional();
            Property(c => c.Salary).IsOptional();
            Property(c => c.TotalProfit).IsOptional();
        }
    }
}