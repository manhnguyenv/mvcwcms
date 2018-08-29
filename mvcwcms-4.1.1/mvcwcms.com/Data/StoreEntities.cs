using MVCwCMS.Configurations;
using MVCwCMS.Models;
using System.Data.Entity;

namespace MVCwCMS.Data
{
    public class StoreEntities : DbContext
    {
        public StoreEntities() : base("StoreEntities")
        {
        }

        public DbSet<Gadget> Gadgets { get; set; }
        public DbSet<Category> Categories { get; set; }

        public virtual void Commit()
        {
            base.SaveChanges();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new GadgetConfiguration());
            modelBuilder.Configurations.Add(new CategoryConfiguration());
        }
    }
}