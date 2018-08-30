using MVCwCMS.Models;
using System.Data.Entity.ModelConfiguration;

namespace MVCwCMS.Configurations
{
    /// <summary>
    /// TODO: Manh
    /// </summary>
    public class ProductConfiguration : EntityTypeConfiguration<Product>
    {
        public ProductConfiguration()
        {
            ToTable("Product");
            Property(c => c.Name).IsRequired().HasMaxLength(255);
            Property(c => c.Price).IsRequired();
            Property(c => c.CategoryId).IsOptional();
            Property(c => c.Code).IsOptional().HasMaxLength(10);
            Property(c => c.Description).IsOptional().HasMaxLength(255);
            Property(c => c.ImageUrl).IsOptional().HasMaxLength(255);
        }
    }
}