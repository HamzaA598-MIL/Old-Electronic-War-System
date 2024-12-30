namespace ElecWarSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class personState : DbMigration
    {
        public override void Up()
        {
            AlterColumn("FileShare.Person", "Status", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("FileShare.Person", "Status", c => c.Int(nullable: false));
        }
    }
}
