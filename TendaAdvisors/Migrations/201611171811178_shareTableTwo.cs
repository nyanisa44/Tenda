namespace TendaAdvisors.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class shareTableTwo : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.AdvisorShareUnderSupervisions");
            AlterColumn("dbo.AdvisorShareUnderSupervisions", "AdvisorId", c => c.Int(nullable: false));
            AlterColumn("dbo.AdvisorShareUnderSupervisions", "LicenseTypeId", c => c.Int(nullable: false));
            AddPrimaryKey("dbo.AdvisorShareUnderSupervisions", new[] { "AdvisorId", "LicenseTypeId", "supplier", "product" });
        }
        
        public override void Down()
        {
            DropPrimaryKey("dbo.AdvisorShareUnderSupervisions");
            AlterColumn("dbo.AdvisorShareUnderSupervisions", "LicenseTypeId", c => c.String(nullable: false, maxLength: 128));
            AlterColumn("dbo.AdvisorShareUnderSupervisions", "AdvisorId", c => c.String(nullable: false, maxLength: 128));
            AddPrimaryKey("dbo.AdvisorShareUnderSupervisions", new[] { "AdvisorId", "LicenseTypeId", "supplier", "product" });
        }
    }
}
