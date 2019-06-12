namespace TendaAdvisors.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedReasonsToUnmatchedCommissions : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UnmatchedCommissions", "Reasons", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.UnmatchedCommissions", "Reasons");
        }
    }
}
