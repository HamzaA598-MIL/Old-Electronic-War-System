namespace ElecWarSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class dummy : DbMigration
    {
        public override void Up()
        {
            DropIndex("FileShare.Users", new[] { "UnitID" });
            CreateIndex("FileShare.Users", "UnitID");
        }
        
        public override void Down()
        {
            DropIndex("FileShare.Users", new[] { "UnitID" });
            CreateIndex("FileShare.Users", "UnitID", unique: true);
        }
    }
}
