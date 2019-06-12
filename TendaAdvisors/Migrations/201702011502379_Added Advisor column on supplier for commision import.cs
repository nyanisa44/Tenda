namespace TendaAdvisors.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedAdvisorcolumnonsupplierforcommisionimport : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Suppliers", "AdvisorColumn", c => c.Int(nullable: false));
            AddColumn("dbo.Suppliers", "AdvisorColumnName", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Suppliers", "AdvisorColumnName");
            DropColumn("dbo.Suppliers", "AdvisorColumn");
        }
    }
}
