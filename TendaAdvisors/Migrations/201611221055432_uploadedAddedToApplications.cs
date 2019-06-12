namespace TendaAdvisors.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class uploadedAddedToApplications : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ApplicationDocuments", "Uploaded", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ApplicationDocuments", "Uploaded");
        }
    }
}
