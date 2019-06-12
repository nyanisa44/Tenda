namespace TendaAdvisors.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class unmatchedCommissionsApplicationdb : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.UnmatchedCommissions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        MemberGroupCode = c.String(),
                        Surname = c.String(),
                        Initial = c.String(),
                        MemberNumber = c.String(),
                        MemberSearchKey = c.String(),
                        MemberSearchValue = c.String(),
                        CommisionRunDate = c.DateTime(),
                        CommisionRunUser = c.String(),
                        EnrollmentDate = c.DateTime(),
                        TerminationDate = c.DateTime(),
                        TransactionDate = c.DateTime(),
                        SubscriptionDue = c.Decimal(precision: 18, scale: 2),
                        SubscriptionReceived = c.Decimal(precision: 18, scale: 2),
                        CommissionInclVAT = c.Decimal(precision: 18, scale: 2),
                        CommissionExclVAT = c.Decimal(precision: 18, scale: 2),
                        AdvisorCommission = c.Decimal(precision: 18, scale: 2),
                        CompanyCommission = c.Decimal(precision: 18, scale: 2),
                        AdvisorTax = c.Decimal(precision: 18, scale: 2),
                        AdvisorTaxRate = c.Decimal(precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.UnmatchedCommissions");
        }
    }
}
