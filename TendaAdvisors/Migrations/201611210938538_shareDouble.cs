namespace TendaAdvisors.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class shareDouble : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.AdvisorShareUnderSupervisions", "Share", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.AdvisorShareUnderSupervisions", "Share", c => c.Int(nullable: false));
        }
    }
}
