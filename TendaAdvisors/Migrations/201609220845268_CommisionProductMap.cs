namespace TendaAdvisors.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CommisionProductMap : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Products", "IDNumberColumn", c => c.Int(nullable: false));
            AddColumn("dbo.Products", "IDNumberColumnName", c => c.String());
            AddColumn("dbo.Products", "PolicyNumberColumn", c => c.Int(nullable: false));
            AddColumn("dbo.Products", "PolicyNumberColumnName", c => c.String());
            AddColumn("dbo.Products", "MemberNumberColumn", c => c.Int(nullable: false));
            AddColumn("dbo.Products", "MemberNumberColumnName", c => c.String());
            DropColumn("dbo.Products", "MemberSearchColumn");
            DropColumn("dbo.Products", "MemberSearchColumnName");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Products", "MemberSearchColumnName", c => c.String());
            AddColumn("dbo.Products", "MemberSearchColumn", c => c.Int(nullable: false));
            DropColumn("dbo.Products", "MemberNumberColumnName");
            DropColumn("dbo.Products", "MemberNumberColumn");
            DropColumn("dbo.Products", "PolicyNumberColumnName");
            DropColumn("dbo.Products", "PolicyNumberColumn");
            DropColumn("dbo.Products", "IDNumberColumnName");
            DropColumn("dbo.Products", "IDNumberColumn");
        }
    }
}
