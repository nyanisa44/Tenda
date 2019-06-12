namespace TendaAdvisors.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Banking : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Advisors", "BankName", c => c.String());
            AddColumn("dbo.Advisors", "BranchCode", c => c.String());
            AddColumn("dbo.Advisors", "AccountType", c => c.String());
            AddColumn("dbo.Advisors", "AccountNumber", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Advisors", "AccountNumber");
            DropColumn("dbo.Advisors", "AccountType");
            DropColumn("dbo.Advisors", "BranchCode");
            DropColumn("dbo.Advisors", "BankName");
        }
    }
}
