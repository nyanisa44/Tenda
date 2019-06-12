namespace TendaAdvisors.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CollectionAdvisorLicenseType : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.LicenseTypes", "Advisor_Id", "dbo.Advisors");
            DropIndex("dbo.LicenseTypes", new[] { "Advisor_Id" });
            CreateTable(
                "dbo.LicenseTypeAdvisors",
                c => new
                    {
                        LicenseType_Id = c.Int(nullable: false),
                        Advisor_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.LicenseType_Id, t.Advisor_Id })
                .ForeignKey("dbo.LicenseTypes", t => t.LicenseType_Id, cascadeDelete: true)
                .ForeignKey("dbo.Advisors", t => t.Advisor_Id, cascadeDelete: true)
                .Index(t => t.LicenseType_Id)
                .Index(t => t.Advisor_Id);
            
            DropColumn("dbo.LicenseTypes", "Advisor_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.LicenseTypes", "Advisor_Id", c => c.Int());
            DropForeignKey("dbo.LicenseTypeAdvisors", "Advisor_Id", "dbo.Advisors");
            DropForeignKey("dbo.LicenseTypeAdvisors", "LicenseType_Id", "dbo.LicenseTypes");
            DropIndex("dbo.LicenseTypeAdvisors", new[] { "Advisor_Id" });
            DropIndex("dbo.LicenseTypeAdvisors", new[] { "LicenseType_Id" });
            DropTable("dbo.LicenseTypeAdvisors");
            CreateIndex("dbo.LicenseTypes", "Advisor_Id");
            AddForeignKey("dbo.LicenseTypes", "Advisor_Id", "dbo.Advisors", "Id");
        }
    }
}
