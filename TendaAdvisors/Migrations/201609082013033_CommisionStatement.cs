namespace TendaAdvisors.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CommisionStatement : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CommissionStatements", "Share", c => c.Int());
            AddColumn("dbo.CommissionStatements", "ProductId", c => c.Int(nullable: false));
            AlterColumn("dbo.CommissionStatements", "AdvisorId", c => c.Int());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.CommissionStatements", "AdvisorId", c => c.Int(nullable: false));
            DropColumn("dbo.CommissionStatements", "ProductId");
            DropColumn("dbo.CommissionStatements", "Share");
        }
    }
}
