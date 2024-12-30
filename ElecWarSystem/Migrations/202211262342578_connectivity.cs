namespace ElecWarSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class connectivity : DbMigration
    {
        public override void Up()
        {
            AddColumn("FileShare.Tmam", "Connectivity", c => c.Byte(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("FileShare.Tmam", "Connectivity");
        }
    }
}
