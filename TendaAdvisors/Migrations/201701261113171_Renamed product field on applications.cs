namespace TendaAdvisors.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Renamedproductfieldonapplications : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.Applications", name: "Products_Id", newName: "Product_Id");
            RenameIndex(table: "dbo.Applications", name: "IX_Products_Id", newName: "IX_Product_Id");
        }
        
        public override void Down()
        {
            RenameIndex(table: "dbo.Applications", name: "IX_Product_Id", newName: "IX_Products_Id");
            RenameColumn(table: "dbo.Applications", name: "Product_Id", newName: "Products_Id");
        }
    }
}
