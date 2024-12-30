namespace ElecWarSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class escapeTmam : DbMigration
    {
        public override void Up()
        {
            AddColumn("FileShare.TmamDetails", "escape", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("FileShare.TmamDetails", "escape");
        }
    }
}
