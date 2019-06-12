namespace TendaAdvisors.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedforeignkeyfieldtoApplication : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ProductApplications", "Product_Id", "dbo.Products");
            DropForeignKey("dbo.ProductApplications", "Application_Id", "dbo.Applications");
            DropIndex("dbo.ProductApplications", new[] { "Product_Id" });
            DropIndex("dbo.ProductApplications", new[] { "Application_Id" });
            AddColumn("dbo.Applications", "Products_Id", c => c.Int());
            CreateIndex("dbo.Applications", "Products_Id");
            AddForeignKey("dbo.Applications", "Products_Id", "dbo.Products", "Id");
            DropTable("dbo.ProductApplications");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.ProductApplications",
                c => new
                    {
                        Product_Id = c.Int(nullable: false),
                        Application_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Product_Id, t.Application_Id });
            
            DropForeignKey("dbo.Applications", "Products_Id", "dbo.Products");
            DropIndex("dbo.Applications", new[] { "Products_Id" });
            DropColumn("dbo.Applications", "Products_Id");
            CreateIndex("dbo.ProductApplications", "Application_Id");
            CreateIndex("dbo.ProductApplications", "Product_Id");
            AddForeignKey("dbo.ProductApplications", "Application_Id", "dbo.Applications", "Id", cascadeDelete: true);
            AddForeignKey("dbo.ProductApplications", "Product_Id", "dbo.Products", "Id", cascadeDelete: true);
        }
    }
}
