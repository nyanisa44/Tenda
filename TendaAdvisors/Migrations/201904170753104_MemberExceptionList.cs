namespace TendaAdvisors.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MemberExceptionList : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.MemberListExceptions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        MemberId = c.String(),
                        Initials = c.String(),
                        MemberSurname = c.String(),
                        IdNumber = c.String(),
                        DateCreated = c.DateTime(nullable: false),
                        Reason = c.String(),
                    })
                .PrimaryKey(t => t.Id);
        }
        
        public override void Down()
        {
            DropTable("dbo.MemberListExceptions");
        }
    }
}
