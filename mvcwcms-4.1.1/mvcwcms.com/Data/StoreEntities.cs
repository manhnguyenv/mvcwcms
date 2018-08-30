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
            //Database.SetInitializer(new DropCreateDatabaseAlways<StoreEntities>());
        }

        public DbSet<Gadget> Gadgets { get; set; }
        public DbSet<Category> Categories { get; set; }

        public DbSet<ProductCategory> ProductCategories { get; set; }
        public DbSet<Product> Products { get; set; } //TODO: Manh

        public virtual void Commit()
        {
            base.SaveChanges();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new GadgetConfiguration());
            modelBuilder.Configurations.Add(new CategoryConfiguration());

            modelBuilder.Configurations.Add(new ProductCategoryConfiguration());
            modelBuilder.Configurations.Add(new ProductConfiguration()); //TODO: Manh
        }
    }
}