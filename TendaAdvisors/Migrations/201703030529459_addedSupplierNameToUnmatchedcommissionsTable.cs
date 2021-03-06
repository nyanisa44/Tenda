namespace TendaAdvisors.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedSupplierNameToUnmatchedcommissionsTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UnmatchedCommissions", "supplierName", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.UnmatchedCommissions", "supplierName");
        }
    }
}
