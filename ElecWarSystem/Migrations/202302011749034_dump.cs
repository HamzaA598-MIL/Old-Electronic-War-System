namespace ElecWarSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class dump : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("FileShare.CourseDetails", "CommandItemID", "FileShare.CommandItems");
            DropForeignKey("FileShare.CourseDetails", "PersonID", "FileShare.Person");
            DropForeignKey("FileShare.Course", "CourseDetailsID", "FileShare.CourseDetails");
            DropForeignKey("FileShare.Course", "TmamID", "FileShare.Tmam");
            DropIndex("FileShare.CourseDetails", new[] { "CommandItemID" });
            DropIndex("FileShare.CourseDetails", new[] { "PersonID" });
            DropIndex("FileShare.Course", new[] { "CourseDetailsID" });
            DropIndex("FileShare.Course", new[] { "TmamID" });
            RenameColumn(table: "FileShare.Vacations", name: "VacationDetailsID", newName: "VacationDetailID");
            RenameIndex(table: "FileShare.Vacations", name: "IX_VacationDetailsID", newName: "IX_VacationDetailID");
            DropColumn("FileShare.TmamDetails", "course");
            DropTable("FileShare.CourseDetails");
            DropTable("FileShare.Course");
        }
        
        public override void Down()
        {
            CreateTable(
                "FileShare.Course",
                c => new
                    {
                        ID = c.Long(nullable: false, identity: true),
                        CourseDetailsID = c.Long(nullable: false),
                        TmamID = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
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
                .PrimaryKey(t => t.ID);
            
            AddColumn("FileShare.TmamDetails", "course", c => c.Int(nullable: false));
            RenameIndex(table: "FileShare.Vacations", name: "IX_VacationDetailID", newName: "IX_VacationDetailsID");
            RenameColumn(table: "FileShare.Vacations", name: "VacationDetailID", newName: "VacationDetailsID");
            CreateIndex("FileShare.Course", "TmamID");
            CreateIndex("FileShare.Course", "CourseDetailsID");
            CreateIndex("FileShare.CourseDetails", "PersonID");
            CreateIndex("FileShare.CourseDetails", "CommandItemID");
            AddForeignKey("FileShare.Course", "TmamID", "FileShare.Tmam", "ID", cascadeDelete: true);
            AddForeignKey("FileShare.Course", "CourseDetailsID", "FileShare.CourseDetails", "ID", cascadeDelete: true);
            AddForeignKey("FileShare.CourseDetails", "PersonID", "FileShare.Person", "ID", cascadeDelete: true);
            AddForeignKey("FileShare.CourseDetails", "CommandItemID", "FileShare.CommandItems", "ID", cascadeDelete: true);
        }
    }
}
