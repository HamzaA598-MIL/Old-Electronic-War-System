namespace ElecWarSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Coursedetails : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "FileShare.Course",
                c => new
                    {
                        ID = c.Long(nullable: false, identity: true),
                        CourseDetailsID = c.Long(nullable: false),
                        TmamID = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("FileShare.CourseDetails", t => t.CourseDetailsID, cascadeDelete: true)
                .ForeignKey("FileShare.Tmam", t => t.TmamID, cascadeDelete: true)
                .Index(t => t.CourseDetailsID)
                .Index(t => t.TmamID);
            
            CreateTable(
                "FileShare.CourseDetails",
                c => new
                    {
                        ID = c.Long(nullable: false, identity: true),
                        CourseName = c.String(),
                        CoursePlace = c.String(),
                        CommandItemID = c.Long(nullable: false),
                        PersonID = c.Long(nullable: false),
                        DateFrom = c.DateTime(nullable: false),
                        DateTo = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("FileShare.CommandItems", t => t.CommandItemID, cascadeDelete: true)
                .ForeignKey("FileShare.Person", t => t.PersonID, cascadeDelete: true)
                .Index(t => t.CommandItemID)
                .Index(t => t.PersonID);
            
            AddColumn("FileShare.TmamDetails", "course", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropForeignKey("FileShare.Course", "TmamID", "FileShare.Tmam");
            DropForeignKey("FileShare.Course", "CourseDetailsID", "FileShare.CourseDetails");
            DropForeignKey("FileShare.CourseDetails", "PersonID", "FileShare.Person");
            DropForeignKey("FileShare.CourseDetails", "CommandItemID", "FileShare.CommandItems");
            DropIndex("FileShare.CourseDetails", new[] { "PersonID" });
            DropIndex("FileShare.CourseDetails", new[] { "CommandItemID" });
            DropIndex("FileShare.Course", new[] { "TmamID" });
            DropIndex("FileShare.Course", new[] { "CourseDetailsID" });
            DropColumn("FileShare.TmamDetails", "course");
            DropTable("FileShare.CourseDetails");
            DropTable("FileShare.Course");
        }
    }
}
