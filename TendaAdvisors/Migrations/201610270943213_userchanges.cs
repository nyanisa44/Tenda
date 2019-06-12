namespace TendaAdvisors.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class userchanges : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Advisors", "User_Username", c => c.String());
            AddColumn("dbo.Advisors", "User_FullName", c => c.String());
            AddColumn("dbo.Advisors", "User_FirstName", c => c.String());
            AddColumn("dbo.Advisors", "User_LastName", c => c.String());
            AddColumn("dbo.Advisors", "User_Password", c => c.String());
            AddColumn("dbo.Advisors", "User_ConfirmPassword", c => c.String());
            AddColumn("dbo.Advisors", "User_Email", c => c.String());
            AddColumn("dbo.Advisors", "User_IsAdmin", c => c.Boolean(nullable: false));
            AddColumn("dbo.Advisors", "User_AdvisorId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Advisors", "User_AdvisorId");
            DropColumn("dbo.Advisors", "User_IsAdmin");
            DropColumn("dbo.Advisors", "User_Email");
            DropColumn("dbo.Advisors", "User_ConfirmPassword");
            DropColumn("dbo.Advisors", "User_Password");
            DropColumn("dbo.Advisors", "User_LastName");
            DropColumn("dbo.Advisors", "User_FirstName");
            DropColumn("dbo.Advisors", "User_FullName");
            DropColumn("dbo.Advisors", "User_Username");
        }
    }
}
