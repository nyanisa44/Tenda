namespace TendaAdvisors.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ComissionFileStatus : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CommissionFileStatus",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserUid = c.String(),
                        FileName = c.String(),
                        Location = c.String(),
                        Size = c.Int(nullable: false),
                        ImportFileId = c.Int(nullable: false),
                        Status = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ImportFiles", t => t.ImportFileId, cascadeDelete: true)
                .Index(t => t.ImportFileId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CommissionFileStatus", "ImportFileId", "dbo.ImportFiles");
            DropIndex("dbo.CommissionFileStatus", new[] { "ImportFileId" });
            DropTable("dbo.CommissionFileStatus");
        }
    }
}
