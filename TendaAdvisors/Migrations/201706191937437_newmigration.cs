namespace TendaAdvisors.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class newmigration : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CommissionFileStatus", "ComissionRunDateFrom", c => c.DateTime());
            AddColumn("dbo.CommissionFileStatus", "ComissionRunDateTo", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.CommissionFileStatus", "ComissionRunDateTo");
            DropColumn("dbo.CommissionFileStatus", "ComissionRunDateFrom");
        }
    }
}
