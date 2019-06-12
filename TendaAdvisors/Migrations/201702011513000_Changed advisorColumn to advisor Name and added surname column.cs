namespace TendaAdvisors.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangedadvisorColumntoadvisorNameandaddedsurnamecolumn : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Suppliers", "AdvisorNameColumn", c => c.Int(nullable: false));
            AddColumn("dbo.Suppliers", "AdvisorNameColumnName", c => c.String());
            AddColumn("dbo.Suppliers", "AdvisorSurnameColumn", c => c.Int(nullable: false));
            AddColumn("dbo.Suppliers", "AdvisorSurnameColumnName", c => c.String());
            DropColumn("dbo.Suppliers", "AdvisorColumn");
            DropColumn("dbo.Suppliers", "AdvisorColumnName");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Suppliers", "AdvisorColumnName", c => c.String());
            AddColumn("dbo.Suppliers", "AdvisorColumn", c => c.Int(nullable: false));
            DropColumn("dbo.Suppliers", "AdvisorSurnameColumnName");
            DropColumn("dbo.Suppliers", "AdvisorSurnameColumn");
            DropColumn("dbo.Suppliers", "AdvisorNameColumnName");
            DropColumn("dbo.Suppliers", "AdvisorNameColumn");
        }
    }
}
