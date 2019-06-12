namespace TendaAdvisors.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class contactTtiles : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ContactTitles",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.Contacts", "ContactTitle_Id", c => c.Int());
            CreateIndex("dbo.Contacts", "ContactTitle_Id");
            AddForeignKey("dbo.Contacts", "ContactTitle_Id", "dbo.ContactTitles", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Contacts", "ContactTitle_Id", "dbo.ContactTitles");
            DropIndex("dbo.Contacts", new[] { "ContactTitle_Id" });
            DropColumn("dbo.Contacts", "ContactTitle_Id");
            DropTable("dbo.ContactTitles");
        }
    }
}
