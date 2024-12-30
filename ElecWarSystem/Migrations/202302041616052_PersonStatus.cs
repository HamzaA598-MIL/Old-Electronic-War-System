namespace ElecWarSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PersonStatus : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "FileShare.PersonStatus",
                c => new
                    {
                        ID = c.Long(nullable: false, identity: true),
                        TmamID = c.Long(nullable: false),
                        PersonID = c.Long(nullable: false),
                        Status = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("FileShare.Person", t => t.PersonID, cascadeDelete: true)
                .ForeignKey("FileShare.Tmam", t => t.TmamID, cascadeDelete: true)
                .Index(t => t.TmamID)
                .Index(t => t.PersonID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("FileShare.PersonStatus", "TmamID", "FileShare.Tmam");
            DropForeignKey("FileShare.PersonStatus", "PersonID", "FileShare.Person");
            DropIndex("FileShare.PersonStatus", new[] { "PersonID" });
            DropIndex("FileShare.PersonStatus", new[] { "TmamID" });
            DropTable("FileShare.PersonStatus");
        }
    }
}
