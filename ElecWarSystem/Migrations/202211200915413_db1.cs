namespace ElecWarSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class db1 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("FileShare.LeaderTmams", "PersonID", "FileShare.Person");
            DropForeignKey("FileShare.LeaderTmams", "TmamID", "FileShare.Tmam");
            DropIndex("FileShare.LeaderTmams", new[] { "TmamID" });
            DropIndex("FileShare.LeaderTmams", new[] { "PersonID" });
            DropTable("FileShare.LeaderTmams");
        }
        
        public override void Down()
        {
            CreateTable(
                "FileShare.LeaderTmams",
                c => new
                    {
                        ID = c.Long(nullable: false, identity: true),
                        TmamID = c.Long(nullable: false),
                        TmamType = c.String(),
                        PersonID = c.Long(nullable: false),
                        DateFrom = c.DateTime(nullable: false),
                        DateTo = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateIndex("FileShare.LeaderTmams", "PersonID");
            CreateIndex("FileShare.LeaderTmams", "TmamID");
            AddForeignKey("FileShare.LeaderTmams", "TmamID", "FileShare.Tmam", "ID", cascadeDelete: true);
            AddForeignKey("FileShare.LeaderTmams", "PersonID", "FileShare.Person", "ID", cascadeDelete: true);
        }
    }
}
