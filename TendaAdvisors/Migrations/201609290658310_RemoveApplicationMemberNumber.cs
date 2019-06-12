namespace TendaAdvisors.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveApplicationMemberNumber : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Applications", "MemberNumber");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Applications", "MemberNumber", c => c.String());
        }
    }
}
