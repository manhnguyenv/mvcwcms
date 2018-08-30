using MVCwCMS.Models;
using System.Data.Entity.ModelConfiguration;

namespace MVCwCMS.Configurations
{
    public class StoreUserConfiguration : EntityTypeConfiguration<StoreUser>
    {
        public StoreUserConfiguration()
        {
            ToTable("StoreUser");
            Property(c => c.Name).IsRequired().HasMaxLength(255);
            Property(c => c.UserName).IsRequired().HasMaxLength(255); // Chính là UserName sau khi đăng nhập
            Property(c => c.Description).IsOptional().HasMaxLength(255);
            Property(c => c.Address).IsOptional().HasMaxLength(255);
            Property(c => c.Phone).IsOptional().HasMaxLength(255);
            Property(c => c.Team).IsOptional();
        }
    }
}