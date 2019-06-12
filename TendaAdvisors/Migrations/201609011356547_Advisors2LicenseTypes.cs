namespace TendaAdvisors.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Advisors2LicenseTypes : DbMigration
    {
        public override void Up()
        {
            
            CreateTable(
                "dbo.Advisor2LicenceType",
                c => new
                    {
                        AdvisorId = c.Int(nullable: false),
                        LicenseTypeId = c.Int(nullable: false),
                        Share = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.AdvisorId, t.LicenseTypeId });
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Advisor2LicenceType");
        }
    }
}
