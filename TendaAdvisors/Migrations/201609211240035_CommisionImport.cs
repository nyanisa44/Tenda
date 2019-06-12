namespace TendaAdvisors.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CommisionImport : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Advisors", "TaxDirectiveRate", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Products", "MemberGroupCode", c => c.Int(nullable: false));
            AddColumn("dbo.Products", "MemberSearchColumn", c => c.Int(nullable: false));
            AddColumn("dbo.Products", "MemberSearchColumnName", c => c.String());
            AddColumn("dbo.Products", "SurnameColumn", c => c.Int(nullable: false));
            AddColumn("dbo.Products", "SurnameColumnName", c => c.String());
            AddColumn("dbo.Products", "InitialColumn", c => c.Int(nullable: false));
            AddColumn("dbo.Products", "InitialColumnName", c => c.String());
            AddColumn("dbo.Products", "EnrollmentDateColumn", c => c.Int(nullable: false));
            AddColumn("dbo.Products", "EnrollmentDateColumnName", c => c.String());
            AddColumn("dbo.Products", "TerminationDateColumn", c => c.Int(nullable: false));
            AddColumn("dbo.Products", "TerminationDateColumnName", c => c.String());
            AddColumn("dbo.Products", "TransactionDateColumn", c => c.Int(nullable: false));
            AddColumn("dbo.Products", "TransactionDateColumnName", c => c.String());
            AddColumn("dbo.Products", "SubscriptionDueColumn", c => c.Int(nullable: false));
            AddColumn("dbo.Products", "SubscriptionDueColumnName", c => c.String());
            AddColumn("dbo.Products", "SubscriptionReceivedColumn", c => c.Int(nullable: false));
            AddColumn("dbo.Products", "SubscriptionReceivedColumnName", c => c.String());
            AddColumn("dbo.Products", "CommissionColumn", c => c.Int(nullable: false));
            AddColumn("dbo.Products", "CommissionColumnName", c => c.String());
            AddColumn("dbo.Products", "VatColumn", c => c.Int(nullable: false));
            AddColumn("dbo.Products", "VatColumnName", c => c.String());
            AddColumn("dbo.Products", "CommissionTotalColumn", c => c.Int(nullable: false));
            AddColumn("dbo.Products", "CommissionTotalColumnName", c => c.String());
            AddColumn("dbo.CommissionStatements", "MemberSearchKey", c => c.String());
            AddColumn("dbo.CommissionStatements", "MemberSearchValue", c => c.String());
            DropColumn("dbo.CommissionStatements", "IdNumber");
        }
        
        public override void Down()
        {
            AddColumn("dbo.CommissionStatements", "IdNumber", c => c.String());
            DropColumn("dbo.CommissionStatements", "MemberSearchValue");
            DropColumn("dbo.CommissionStatements", "MemberSearchKey");
            DropColumn("dbo.Products", "CommissionTotalColumnName");
            DropColumn("dbo.Products", "CommissionTotalColumn");
            DropColumn("dbo.Products", "VatColumnName");
            DropColumn("dbo.Products", "VatColumn");
            DropColumn("dbo.Products", "CommissionColumnName");
            DropColumn("dbo.Products", "CommissionColumn");
            DropColumn("dbo.Products", "SubscriptionReceivedColumnName");
            DropColumn("dbo.Products", "SubscriptionReceivedColumn");
            DropColumn("dbo.Products", "SubscriptionDueColumnName");
            DropColumn("dbo.Products", "SubscriptionDueColumn");
            DropColumn("dbo.Products", "TransactionDateColumnName");
            DropColumn("dbo.Products", "TransactionDateColumn");
            DropColumn("dbo.Products", "TerminationDateColumnName");
            DropColumn("dbo.Products", "TerminationDateColumn");
            DropColumn("dbo.Products", "EnrollmentDateColumnName");
            DropColumn("dbo.Products", "EnrollmentDateColumn");
            DropColumn("dbo.Products", "InitialColumnName");
            DropColumn("dbo.Products", "InitialColumn");
            DropColumn("dbo.Products", "SurnameColumnName");
            DropColumn("dbo.Products", "SurnameColumn");
            DropColumn("dbo.Products", "MemberSearchColumnName");
            DropColumn("dbo.Products", "MemberSearchColumn");
            DropColumn("dbo.Products", "MemberGroupCode");
            DropColumn("dbo.Advisors", "TaxDirectiveRate");
        }
    }
}
