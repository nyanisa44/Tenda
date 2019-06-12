namespace TendaAdvisors.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class tax : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AnnualTaxBrackets",
                c => new
                    {
                        AnnualTaxBracketId = c.Int(nullable: false, identity: true),
                        year = c.Int(nullable: false),
                        type = c.String(),
                        rate = c.Decimal(nullable: false, precision: 18, scale: 2),
                        MinIncome = c.Decimal(nullable: false, precision: 18, scale: 2),
                        MaxIncome = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Threshold = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Basic = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.AnnualTaxBracketId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.AnnualTaxBrackets");
        }
    }
}
