namespace TendaAdvisors.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class shareTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AdvisorShareUnderSupervisions",
                c => new
                    {
                        AdvisorId = c.String(nullable: false, maxLength: 128),
                        LicenseTypeId = c.String(nullable: false, maxLength: 128),
                        supplier = c.String(nullable: false, maxLength: 128),
                        product = c.String(nullable: false, maxLength: 128),
                        Share = c.Int(nullable: false),
                        underSupervision = c.Boolean(nullable: false),
                        Advisor = c.Int(nullable: false),
                        validCommissionFromDate = c.DateTime(),
                        validCommissionToDate = c.DateTime(),
                        ValidFromDate = c.DateTime(),
                        ValidToDate = c.DateTime(),
                    })
                .PrimaryKey(t => new { t.AdvisorId, t.LicenseTypeId, t.supplier, t.product });
            
        }
        
        public override void Down()
        {
            DropTable("dbo.AdvisorShareUnderSupervisions");
        }
    }
}
