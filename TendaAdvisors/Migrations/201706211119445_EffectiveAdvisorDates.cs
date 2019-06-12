namespace TendaAdvisors.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EffectiveAdvisorDates : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Advisors", "EffectiveStartDate", c => c.DateTime());
            AddColumn("dbo.Advisors", "EffectiveEndDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Advisors", "EffectiveEndDate");
            DropColumn("dbo.Advisors", "EffectiveStartDate");
        }
    }
}
