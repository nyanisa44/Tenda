namespace TendaAdvisors.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ProductCommisionUpdates : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Products", "CommissionExclVatColumn", c => c.Int(nullable: false));
            AddColumn("dbo.Products", "CommissionExclVatColumnName", c => c.String());
            AddColumn("dbo.Products", "CommissionInclVatColumn", c => c.Int(nullable: false));
            AddColumn("dbo.Products", "CommissionInclVatColumnName", c => c.String());
            AddColumn("dbo.CommissionStatements", "CommissionInclVAT", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.CommissionStatements", "CommissionExclVAT", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.CommissionStatements", "AdvisorCommission", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.CommissionStatements", "CompanyCommission", c => c.Decimal(precision: 18, scale: 2));
            DropColumn("dbo.Products", "CommissionColumn");
            DropColumn("dbo.Products", "CommissionColumnName");
            DropColumn("dbo.Products", "VatColumn");
            DropColumn("dbo.Products", "VatColumnName");
            DropColumn("dbo.Products", "CommissionTotalColumn");
            DropColumn("dbo.Products", "CommissionTotalColumnName");
            DropColumn("dbo.CommissionStatements", "Commission");
            DropColumn("dbo.CommissionStatements", "Vat");
            DropColumn("dbo.CommissionStatements", "CommissionTotal");
            DropColumn("dbo.CommissionStatements", "AdvisorShare");
            DropColumn("dbo.CommissionStatements", "CompanyShare");
        }
        
        public override void Down()
        {
            AddColumn("dbo.CommissionStatements", "CompanyShare", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.CommissionStatements", "AdvisorShare", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.CommissionStatements", "CommissionTotal", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.CommissionStatements", "Vat", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.CommissionStatements", "Commission", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.Products", "CommissionTotalColumnName", c => c.String());
            AddColumn("dbo.Products", "CommissionTotalColumn", c => c.Int(nullable: false));
            AddColumn("dbo.Products", "VatColumnName", c => c.String());
            AddColumn("dbo.Products", "VatColumn", c => c.Int(nullable: false));
            AddColumn("dbo.Products", "CommissionColumnName", c => c.String());
            AddColumn("dbo.Products", "CommissionColumn", c => c.Int(nullable: false));
            DropColumn("dbo.CommissionStatements", "CompanyCommission");
            DropColumn("dbo.CommissionStatements", "AdvisorCommission");
            DropColumn("dbo.CommissionStatements", "CommissionExclVAT");
            DropColumn("dbo.CommissionStatements", "CommissionInclVAT");
            DropColumn("dbo.Products", "CommissionInclVatColumnName");
            DropColumn("dbo.Products", "CommissionInclVatColumn");
            DropColumn("dbo.Products", "CommissionExclVatColumnName");
            DropColumn("dbo.Products", "CommissionExclVatColumn");
        }
    }
}
