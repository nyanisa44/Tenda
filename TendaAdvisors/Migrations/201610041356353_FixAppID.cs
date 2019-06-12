namespace TendaAdvisors.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FixAppID : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DocumentTypes", "ApplicationType_Id", c => c.Int(nullable: false));
            AddColumn("dbo.Suppliers", "MemberGroupCode", c => c.Int(nullable: false));
            AddColumn("dbo.Suppliers", "IDNumberColumn", c => c.Int(nullable: false));
            AddColumn("dbo.Suppliers", "IDNumberColumnName", c => c.String());
            AddColumn("dbo.Suppliers", "PolicyNumberColumn", c => c.Int(nullable: false));
            AddColumn("dbo.Suppliers", "PolicyNumberColumnName", c => c.String());
            AddColumn("dbo.Suppliers", "MemberNumberColumn", c => c.Int(nullable: false));
            AddColumn("dbo.Suppliers", "MemberNumberColumnName", c => c.String());
            AddColumn("dbo.Suppliers", "SurnameColumn", c => c.Int(nullable: false));
            AddColumn("dbo.Suppliers", "SurnameColumnName", c => c.String());
            AddColumn("dbo.Suppliers", "InitialColumn", c => c.Int(nullable: false));
            AddColumn("dbo.Suppliers", "InitialColumnName", c => c.String());
            AddColumn("dbo.Suppliers", "EnrollmentDateColumn", c => c.Int(nullable: false));
            AddColumn("dbo.Suppliers", "EnrollmentDateColumnName", c => c.String());
            AddColumn("dbo.Suppliers", "TerminationDateColumn", c => c.Int(nullable: false));
            AddColumn("dbo.Suppliers", "TerminationDateColumnName", c => c.String());
            AddColumn("dbo.Suppliers", "TransactionDateColumn", c => c.Int(nullable: false));
            AddColumn("dbo.Suppliers", "TransactionDateColumnName", c => c.String());
            AddColumn("dbo.Suppliers", "SubscriptionDueColumn", c => c.Int(nullable: false));
            AddColumn("dbo.Suppliers", "SubscriptionDueColumnName", c => c.String());
            AddColumn("dbo.Suppliers", "SubscriptionReceivedColumn", c => c.Int(nullable: false));
            AddColumn("dbo.Suppliers", "SubscriptionReceivedColumnName", c => c.String());
            AddColumn("dbo.Suppliers", "CommissionExclVatColumn", c => c.Int(nullable: false));
            AddColumn("dbo.Suppliers", "CommissionExclVatColumnName", c => c.String());
            AddColumn("dbo.Suppliers", "CommissionInclVatColumn", c => c.Int(nullable: false));
            AddColumn("dbo.Suppliers", "CommissionInclVatColumnName", c => c.String());
            DropColumn("dbo.Products", "MemberGroupCode");
            DropColumn("dbo.Products", "IDNumberColumn");
            DropColumn("dbo.Products", "IDNumberColumnName");
            DropColumn("dbo.Products", "PolicyNumberColumn");
            DropColumn("dbo.Products", "PolicyNumberColumnName");
            DropColumn("dbo.Products", "MemberNumberColumn");
            DropColumn("dbo.Products", "MemberNumberColumnName");
            DropColumn("dbo.Products", "SurnameColumn");
            DropColumn("dbo.Products", "SurnameColumnName");
            DropColumn("dbo.Products", "InitialColumn");
            DropColumn("dbo.Products", "InitialColumnName");
            DropColumn("dbo.Products", "EnrollmentDateColumn");
            DropColumn("dbo.Products", "EnrollmentDateColumnName");
            DropColumn("dbo.Products", "TerminationDateColumn");
            DropColumn("dbo.Products", "TerminationDateColumnName");
            DropColumn("dbo.Products", "TransactionDateColumn");
            DropColumn("dbo.Products", "TransactionDateColumnName");
            DropColumn("dbo.Products", "SubscriptionDueColumn");
            DropColumn("dbo.Products", "SubscriptionDueColumnName");
            DropColumn("dbo.Products", "SubscriptionReceivedColumn");
            DropColumn("dbo.Products", "SubscriptionReceivedColumnName");
            DropColumn("dbo.Products", "CommissionExclVatColumn");
            DropColumn("dbo.Products", "CommissionExclVatColumnName");
            DropColumn("dbo.Products", "CommissionInclVatColumn");
            DropColumn("dbo.Products", "CommissionInclVatColumnName");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Products", "CommissionInclVatColumnName", c => c.String());
            AddColumn("dbo.Products", "CommissionInclVatColumn", c => c.Int(nullable: false));
            AddColumn("dbo.Products", "CommissionExclVatColumnName", c => c.String());
            AddColumn("dbo.Products", "CommissionExclVatColumn", c => c.Int(nullable: false));
            AddColumn("dbo.Products", "SubscriptionReceivedColumnName", c => c.String());
            AddColumn("dbo.Products", "SubscriptionReceivedColumn", c => c.Int(nullable: false));
            AddColumn("dbo.Products", "SubscriptionDueColumnName", c => c.String());
            AddColumn("dbo.Products", "SubscriptionDueColumn", c => c.Int(nullable: false));
            AddColumn("dbo.Products", "TransactionDateColumnName", c => c.String());
            AddColumn("dbo.Products", "TransactionDateColumn", c => c.Int(nullable: false));
            AddColumn("dbo.Products", "TerminationDateColumnName", c => c.String());
            AddColumn("dbo.Products", "TerminationDateColumn", c => c.Int(nullable: false));
            AddColumn("dbo.Products", "EnrollmentDateColumnName", c => c.String());
            AddColumn("dbo.Products", "EnrollmentDateColumn", c => c.Int(nullable: false));
            AddColumn("dbo.Products", "InitialColumnName", c => c.String());
            AddColumn("dbo.Products", "InitialColumn", c => c.Int(nullable: false));
            AddColumn("dbo.Products", "SurnameColumnName", c => c.String());
            AddColumn("dbo.Products", "SurnameColumn", c => c.Int(nullable: false));
            AddColumn("dbo.Products", "MemberNumberColumnName", c => c.String());
            AddColumn("dbo.Products", "MemberNumberColumn", c => c.Int(nullable: false));
            AddColumn("dbo.Products", "PolicyNumberColumnName", c => c.String());
            AddColumn("dbo.Products", "PolicyNumberColumn", c => c.Int(nullable: false));
            AddColumn("dbo.Products", "IDNumberColumnName", c => c.String());
            AddColumn("dbo.Products", "IDNumberColumn", c => c.Int(nullable: false));
            AddColumn("dbo.Products", "MemberGroupCode", c => c.Int(nullable: false));
            DropColumn("dbo.Suppliers", "CommissionInclVatColumnName");
            DropColumn("dbo.Suppliers", "CommissionInclVatColumn");
            DropColumn("dbo.Suppliers", "CommissionExclVatColumnName");
            DropColumn("dbo.Suppliers", "CommissionExclVatColumn");
            DropColumn("dbo.Suppliers", "SubscriptionReceivedColumnName");
            DropColumn("dbo.Suppliers", "SubscriptionReceivedColumn");
            DropColumn("dbo.Suppliers", "SubscriptionDueColumnName");
            DropColumn("dbo.Suppliers", "SubscriptionDueColumn");
            DropColumn("dbo.Suppliers", "TransactionDateColumnName");
            DropColumn("dbo.Suppliers", "TransactionDateColumn");
            DropColumn("dbo.Suppliers", "TerminationDateColumnName");
            DropColumn("dbo.Suppliers", "TerminationDateColumn");
            DropColumn("dbo.Suppliers", "EnrollmentDateColumnName");
            DropColumn("dbo.Suppliers", "EnrollmentDateColumn");
            DropColumn("dbo.Suppliers", "InitialColumnName");
            DropColumn("dbo.Suppliers", "InitialColumn");
            DropColumn("dbo.Suppliers", "SurnameColumnName");
            DropColumn("dbo.Suppliers", "SurnameColumn");
            DropColumn("dbo.Suppliers", "MemberNumberColumnName");
            DropColumn("dbo.Suppliers", "MemberNumberColumn");
            DropColumn("dbo.Suppliers", "PolicyNumberColumnName");
            DropColumn("dbo.Suppliers", "PolicyNumberColumn");
            DropColumn("dbo.Suppliers", "IDNumberColumnName");
            DropColumn("dbo.Suppliers", "IDNumberColumn");
            DropColumn("dbo.Suppliers", "MemberGroupCode");
            DropColumn("dbo.DocumentTypes", "ApplicationType_Id");
        }
    }
}
