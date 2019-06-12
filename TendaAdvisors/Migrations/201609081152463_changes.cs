namespace TendaAdvisors.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class changes : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Advisors", "RegNumber", c => c.String());
            AddColumn("dbo.Applications", "MemberNumber", c => c.String());
            AddColumn("dbo.CommissionStatements", "IdNumber", c => c.String());
            DropColumn("dbo.Contacts", "MemberNumber");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Contacts", "MemberNumber", c => c.String());
            DropColumn("dbo.CommissionStatements", "IdNumber");
            DropColumn("dbo.Applications", "MemberNumber");
            DropColumn("dbo.Advisors", "RegNumber");
        }
    }
}
