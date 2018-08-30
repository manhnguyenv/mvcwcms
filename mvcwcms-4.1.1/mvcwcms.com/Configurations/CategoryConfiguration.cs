using MVCwCMS.Models;
using System.Data.Entity.ModelConfiguration;

namespace MVCwCMS.Configurations
{
    public class CategoryConfiguration : EntityTypeConfiguration<Category>
    {
        public CategoryConfiguration()
        {
            ToTable("Category");
            Property(c => c.Name).IsRequired().HasMaxLength(255);
        }
    }
}