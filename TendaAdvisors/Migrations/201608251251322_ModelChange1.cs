namespace TendaAdvisors.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModelChange1 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Contacts", "LastName", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Contacts", "LastName", c => c.String());
        }
    }
}
