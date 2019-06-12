namespace TendaAdvisors.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class advisorBankNames : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.BankNames",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.Advisors", "BankName_Id", c => c.Int());
            CreateIndex("dbo.Advisors", "BankName_Id");
            AddForeignKey("dbo.Advisors", "BankName_Id", "dbo.BankNames", "Id");
            DropColumn("dbo.Advisors", "BankName");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Advisors", "BankName", c => c.String());
            DropForeignKey("dbo.Advisors", "BankName_Id", "dbo.BankNames");
            DropIndex("dbo.Advisors", new[] { "BankName_Id" });
            DropColumn("dbo.Advisors", "BankName_Id");
            DropTable("dbo.BankNames");
        }
    }
}
