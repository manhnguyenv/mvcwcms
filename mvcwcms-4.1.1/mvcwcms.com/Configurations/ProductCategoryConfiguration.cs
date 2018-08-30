using MVCwCMS.Models;
using System.Data.Entity.ModelConfiguration;

namespace MVCwCMS.Configurations
{
    public class ProductCategoryConfiguration : EntityTypeConfiguration<ProductCategory>
    {
        public ProductCategoryConfiguration()
        {
            ToTable("ProductCategory");
            Property(c => c.Name).IsRequired().HasMaxLength(50);
            Property(c => c.ParentId).IsOptional();
            Property(c => c.Description).IsOptional().HasMaxLength(255);
        }
    }
}