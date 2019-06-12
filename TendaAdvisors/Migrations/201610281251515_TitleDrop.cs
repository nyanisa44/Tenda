namespace TendaAdvisors.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TitleDrop : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Contacts", "FirstName", c => c.String(nullable: false, maxLength: 10));
            DropColumn("dbo.Contacts", "Title");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Contacts", "Title", c => c.String(maxLength: 10));
            AlterColumn("dbo.Contacts", "FirstName", c => c.String(nullable: false));
        }
    }
}
