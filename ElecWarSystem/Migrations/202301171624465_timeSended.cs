namespace ElecWarSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class timeSended : DbMigration
    {
        public override void Up()
        {
            AddColumn("FileShare.Tmam", "TimeSended", c => c.String());
            DropColumn("FileShare.Tmam", "Connectivity");
        }
        
        public override void Down()
        {
            AddColumn("FileShare.Tmam", "Connectivity", c => c.Byte(nullable: false));
            DropColumn("FileShare.Tmam", "TimeSended");
        }
    }
}
