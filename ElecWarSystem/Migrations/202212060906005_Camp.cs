namespace ElecWarSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Camp : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CampDetails",
                c => new
                    {
                        ID = c.Long(nullable: false, identity: true),
                        CurrentExistance = c.String(),
                        Reason = c.String(),
                        PersonID = c.Long(nullable: false),
                        DateFrom = c.DateTime(nullable: false),
                        DateTo = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("FileShare.Person", t => t.PersonID, cascadeDelete: true)
                .Index(t => t.PersonID);
            
            CreateTable(
                "dbo.Camps",
                c => new
                    {
                        ID = c.Long(nullable: false, identity: true),
                        CampDetailID = c.Long(nullable: false),
                        TmamID = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.CampDetails", t => t.CampDetailID, cascadeDelete: true)
                .ForeignKey("FileShare.Tmam", t => t.TmamID, cascadeDelete: true)
                .Index(t => t.CampDetailID)
                .Index(t => t.TmamID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Camps", "TmamID", "FileShare.Tmam");
            DropForeignKey("dbo.Camps", "CampDetailID", "dbo.CampDetails");
            DropForeignKey("dbo.CampDetails", "PersonID", "FileShare.Person");
            DropIndex("dbo.Camps", new[] { "TmamID" });
            DropIndex("dbo.Camps", new[] { "CampDetailID" });
            DropIndex("dbo.CampDetails", new[] { "PersonID" });
            DropTable("dbo.Camps");
            DropTable("dbo.CampDetails");
        }
    }
}
