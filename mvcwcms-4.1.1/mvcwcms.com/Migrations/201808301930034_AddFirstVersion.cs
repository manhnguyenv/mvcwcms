namespace MVCwCMS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddFirstVersion : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Category",
                c => new
                    {
                        CategoryID = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 255),
                        DateCreated = c.DateTime(),
                        DateUpdated = c.DateTime(),
                    })
                .PrimaryKey(t => t.CategoryID);
            
            CreateTable(
                "dbo.Gadget",
                c => new
                    {
                        GadgetID = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 255),
                        Description = c.String(),
                        Price = c.Decimal(nullable: false, precision: 8, scale: 2),
                        Image = c.String(),
                        CategoryID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.GadgetID)
                .ForeignKey("dbo.Category", t => t.CategoryID, cascadeDelete: true)
                .Index(t => t.CategoryID);
            
            CreateTable(
                "dbo.CustomerGroup",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 255),
                        UserName = c.String(nullable: false, maxLength: 255),
                        CreatedDate = c.DateTime(),
                        ModifiedDate = c.DateTime(),
                        ModifiedBy = c.Guid(),
                        CreatedBy = c.Guid(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Customer",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 255),
                        UserName = c.String(nullable: false, maxLength: 255),
                        Description = c.String(maxLength: 255),
                        Address = c.String(maxLength: 255),
                        Phone = c.String(maxLength: 255),
                        Email = c.String(maxLength: 255),
                        Facebook = c.String(maxLength: 255),
                        Zalo = c.String(maxLength: 255),
                        CustomerGroupId = c.Int(),
                        CreatedDate = c.DateTime(),
                        ModifiedDate = c.DateTime(),
                        ModifiedBy = c.Guid(),
                        CreatedBy = c.Guid(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Discount",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CustomerId = c.Int(nullable: false),
                        Level = c.String(nullable: false, maxLength: 255),
                        Sale = c.Decimal(precision: 18, scale: 2),
                        CK = c.Decimal(precision: 18, scale: 2),
                        Capital = c.Decimal(precision: 18, scale: 2),
                        Profit = c.Decimal(precision: 18, scale: 2),
                        Bonus = c.Decimal(precision: 18, scale: 2),
                        Salary = c.Decimal(precision: 18, scale: 2),
                        TotalProfit = c.Decimal(precision: 18, scale: 2),
                        IsActive = c.Boolean(nullable: false),
                        CreatedDate = c.DateTime(),
                        ModifiedDate = c.DateTime(),
                        ModifiedBy = c.Guid(),
                        CreatedBy = c.Guid(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ProductCategory",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 255),
                        Description = c.String(maxLength: 255),
                        ParentId = c.Int(),
                        CreatedDate = c.DateTime(),
                        ModifiedDate = c.DateTime(),
                        ModifiedBy = c.Guid(),
                        CreatedBy = c.Guid(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Product",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 255),
                        Code = c.String(maxLength: 10),
                        Description = c.String(maxLength: 255),
                        ImageUrl = c.String(maxLength: 255),
                        Price = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CategoryId = c.Int(),
                        CreatedDate = c.DateTime(),
                        ModifiedDate = c.DateTime(),
                        ModifiedBy = c.Guid(),
                        CreatedBy = c.Guid(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Store",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 255),
                        Description = c.String(maxLength: 255),
                        Address = c.String(maxLength: 255),
                        Phone = c.String(maxLength: 255),
                        Owner = c.Int(),
                        CreatedDate = c.DateTime(),
                        ModifiedDate = c.DateTime(),
                        ModifiedBy = c.Guid(),
                        CreatedBy = c.Guid(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.StoreUser",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 255),
                        UserName = c.String(nullable: false, maxLength: 255),
                        Description = c.String(maxLength: 255),
                        Address = c.String(maxLength: 255),
                        Phone = c.String(maxLength: 255),
                        Team = c.Int(),
                        CreatedDate = c.DateTime(),
                        ModifiedDate = c.DateTime(),
                        ModifiedBy = c.Guid(),
                        CreatedBy = c.Guid(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Team",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 255),
                        Slogan = c.String(maxLength: 255),
                        Description = c.String(maxLength: 255),
                        CreatedDate = c.DateTime(),
                        ModifiedDate = c.DateTime(),
                        ModifiedBy = c.Guid(),
                        CreatedBy = c.Guid(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Gadget", "CategoryID", "dbo.Category");
            DropIndex("dbo.Gadget", new[] { "CategoryID" });
            DropTable("dbo.Team");
            DropTable("dbo.StoreUser");
            DropTable("dbo.Store");
            DropTable("dbo.Product");
            DropTable("dbo.ProductCategory");
            DropTable("dbo.Discount");
            DropTable("dbo.Customer");
            DropTable("dbo.CustomerGroup");
            DropTable("dbo.Gadget");
            DropTable("dbo.Category");
        }
    }
}
