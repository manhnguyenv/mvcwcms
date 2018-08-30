using MVCwCMS.Configurations;
using MVCwCMS.Models;
using System.Data.Entity;

namespace MVCwCMS.Data
{
    /// <summary>
    /// TODO: Manh
    /// </summary>
    public class StoreEntities : DbContext
    {
        public StoreEntities() : base("StoreEntities")
        {
        }

        public DbSet<Gadget> Gadgets { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Store> Stores { get; set; }
        public DbSet<StoreUser> StoreUsers { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<CustomerGroup> CustomerGroups { get; set; }
        public DbSet<Discount> Discounts { get; set; }
        public DbSet<ProductCategory> ProductCategories { get; set; }
        public DbSet<Product> Products { get; set; }

        public virtual void Commit()
        {
            base.SaveChanges();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new GadgetConfiguration());
            modelBuilder.Configurations.Add(new CategoryConfiguration());
            modelBuilder.Configurations.Add(new StoreConfiguration());
            modelBuilder.Configurations.Add(new StoreUserConfiguration());
            modelBuilder.Configurations.Add(new TeamConfiguration());
            modelBuilder.Configurations.Add(new CustomerConfiguration());
            modelBuilder.Configurations.Add(new CustomerGroupConfiguration());
            modelBuilder.Configurations.Add(new DiscountConfiguration());
            modelBuilder.Configurations.Add(new ProductCategoryConfiguration());
            modelBuilder.Configurations.Add(new ProductConfiguration());
        }
    }
}