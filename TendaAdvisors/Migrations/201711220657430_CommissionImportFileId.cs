namespace TendaAdvisors.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CommissionImportFileId : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CommissionStatements", "ImportFileId", c => c.Int(nullable:true));
            AddColumn("dbo.UnmatchedCommissions", "ImportFileId", c => c.Int(nullable: true));
            CreateIndex("dbo.CommissionStatements", "ImportFileId");
            CreateIndex("dbo.UnmatchedCommissions", "ImportFileId");
            AddForeignKey("dbo.CommissionStatements", "ImportFileId", "dbo.ImportFiles", "Id");
            AddForeignKey("dbo.UnmatchedCommissions", "ImportFileId", "dbo.ImportFiles", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UnmatchedCommissions", "ImportFileId", "dbo.ImportFiles");
            DropForeignKey("dbo.CommissionStatements", "ImportFileId", "dbo.ImportFiles");
            DropIndex("dbo.UnmatchedCommissions", new[] { "ImportFileId" });
            DropIndex("dbo.CommissionStatements", new[] { "ImportFileId" });
            DropColumn("dbo.UnmatchedCommissions", "ImportFileId");
            DropColumn("dbo.CommissionStatements", "ImportFileId");
        }
    }
}
