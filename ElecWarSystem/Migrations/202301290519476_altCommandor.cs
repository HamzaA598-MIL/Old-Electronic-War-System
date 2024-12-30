namespace ElecWarSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class altCommandor : DbMigration
    {
        public override void Up()
        {
            AddColumn("FileShare.Unit", "AltComExist", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("FileShare.Unit", "AltComExist");
        }
    }
}
