namespace ElecWarSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UserRoles : DbMigration
    {
        public override void Up()
        {
            AddColumn("FileShare.Users", "Roles", c => c.Byte(nullable: false));
            DropTable("FileShare.OfficcerTypes");
            DropTable("FileShare.Weapons");
        }
        
        public override void Down()
        {
            CreateTable(
                "FileShare.Weapons",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "FileShare.OfficcerTypes",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Type = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
            DropColumn("FileShare.Users", "Roles");
        }
    }
}
