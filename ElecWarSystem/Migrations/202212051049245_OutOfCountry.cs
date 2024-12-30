namespace ElecWarSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class OutOfCountry : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "FileShare.OutOfCountries",
                c => new
                    {
                        ID = c.Long(nullable: false, identity: true),
                        OutOfCountryDetailID = c.Long(nullable: false),
                        TmamID = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("FileShare.OutOfCountryDetails", t => t.OutOfCountryDetailID, cascadeDelete: true)
                .ForeignKey("FileShare.Tmam", t => t.TmamID, cascadeDelete: true)
                .Index(t => t.OutOfCountryDetailID)
                .Index(t => t.TmamID);
            
            CreateTable(
                "FileShare.OutOfCountryDetails",
                c => new
                    {
                        ID = c.Long(nullable: false, identity: true),
                        Country = c.String(),
                        Puspose = c.String(),
                        PersonID = c.Long(nullable: false),
                        DateFrom = c.DateTime(nullable: false),
                        DateTo = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("FileShare.Person", t => t.PersonID, cascadeDelete: true)
                .Index(t => t.PersonID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("FileShare.OutOfCountries", "TmamID", "FileShare.Tmam");
            DropForeignKey("FileShare.OutOfCountries", "OutOfCountryDetailID", "FileShare.OutOfCountryDetails");
            DropForeignKey("FileShare.OutOfCountryDetails", "PersonID", "FileShare.Person");
            DropIndex("FileShare.OutOfCountryDetails", new[] { "PersonID" });
            DropIndex("FileShare.OutOfCountries", new[] { "TmamID" });
            DropIndex("FileShare.OutOfCountries", new[] { "OutOfCountryDetailID" });
            DropTable("FileShare.OutOfCountryDetails");
            DropTable("FileShare.OutOfCountries");
        }
    }
}
