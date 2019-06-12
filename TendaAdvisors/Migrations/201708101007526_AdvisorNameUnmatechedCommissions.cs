namespace TendaAdvisors.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AdvisorNameUnmatechedCommissions : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UnmatchedCommissions", "AdvisorName", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.UnmatchedCommissions", "AdvisorName");
        }
    }
}
