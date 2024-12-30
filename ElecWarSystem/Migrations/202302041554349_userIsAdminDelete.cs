namespace ElecWarSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class userIsAdminDelete : DbMigration
    {
        public override void Up()
        {
            DropColumn("FileShare.Users", "isAdmin");
        }
        
        public override void Down()
        {
            AddColumn("FileShare.Users", "isAdmin", c => c.Boolean(nullable: false));
        }
    }
}
