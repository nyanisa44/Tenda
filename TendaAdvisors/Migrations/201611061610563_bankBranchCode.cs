namespace TendaAdvisors.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class bankBranchCode : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.BankBranchCodes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.Advisors", "BranchCode_Id", c => c.Int());
            CreateIndex("dbo.Advisors", "BranchCode_Id");
            AddForeignKey("dbo.Advisors", "BranchCode_Id", "dbo.BankBranchCodes", "Id");
            DropColumn("dbo.Advisors", "BranchCode");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Advisors", "BranchCode", c => c.String());
            DropForeignKey("dbo.Advisors", "BranchCode_Id", "dbo.BankBranchCodes");
            DropIndex("dbo.Advisors", new[] { "BranchCode_Id" });
            DropColumn("dbo.Advisors", "BranchCode_Id");
            DropTable("dbo.BankBranchCodes");
        }
    }
}
