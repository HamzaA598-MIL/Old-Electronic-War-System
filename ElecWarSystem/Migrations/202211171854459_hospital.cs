namespace ElecWarSystem.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class hospital : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "FileShare.HospitalDetails",
                c => new
                {
                    ID = c.Long(nullable: false, identity: true),
                    Hospital = c.String(),
                    Diagnosis = c.String(),
                    Recommendations = c.String(),
                    PersonID = c.Long(nullable: false),
                    DateFrom = c.DateTime(nullable: false),
                    DateTo = c.DateTime(nullable: false),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("FileShare.Person", t => t.PersonID, cascadeDelete: true)
                .Index(t => t.PersonID);

            CreateTable(
                "FileShare.Hospital",
                c => new
                {
                    ID = c.Long(nullable: false, identity: true),
                    HospitalDetailID = c.Long(nullable: false),
                    TmamID = c.Long(nullable: false),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("FileShare.HospitalDetails", t => t.HospitalDetailID, cascadeDelete: true)
                .ForeignKey("FileShare.Tmam", t => t.TmamID, cascadeDelete: true)
                .Index(t => t.HospitalDetailID)
                .Index(t => t.TmamID);

        }

        public override void Down()
        {
            DropForeignKey("FileShare.Hospital", "TmamID", "FileShare.Tmam");
            DropForeignKey("FileShare.Hospital", "HospitalDetailID", "FileShare.PrisonDetails");
            DropForeignKey("FileShare.HospitalDetails", "PersonID", "FileShare.Person");
            DropIndex("FileShare.Hospital", new[] { "TmamID" });
            DropIndex("FileShare.Hospital", new[] { "HospitalDetailID" });
            DropIndex("FileShare.HospitalDetails", new[] { "PersonID" });
            DropTable("FileShare.Hospital");
            DropTable("FileShare.HospitalDetails");
        }
    }
}
