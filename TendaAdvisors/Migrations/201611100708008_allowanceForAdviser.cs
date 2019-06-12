namespace TendaAdvisors.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class allowanceForAdviser : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Advisors", "Allowance", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Advisors", "Allowance");
        }
    }
}
