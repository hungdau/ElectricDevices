namespace TechZone.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class identity : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.IdentityUserClaims", newName: "ApplicationUserClaims");
            RenameTable(name: "dbo.IdentityUserLogins", newName: "ApplicationUserLogins");
            RenameTable(name: "dbo.IdentityUserRoles", newName: "ApplicationUserRoles");
            RenameTable(name: "dbo.IdentityRoles", newName: "ApplicationRoles");
            DropPrimaryKey("dbo.ApplicationUserClaims");
            CreateTable(
                "dbo.ApplicationGroups",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 250),
                        Description = c.String(maxLength: 250),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.ApplicationRoleGroups",
                c => new
                    {
                        GroupId = c.Int(nullable: false),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.GroupId, t.RoleId })
                .ForeignKey("dbo.ApplicationGroups", t => t.GroupId, cascadeDelete: true)
                .ForeignKey("dbo.ApplicationRoles", t => t.RoleId, cascadeDelete: true)
                .Index(t => t.GroupId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.ApplicationUserGroups",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        GroupId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.UserId, t.GroupId })
                .ForeignKey("dbo.ApplicationGroups", t => t.GroupId, cascadeDelete: true)
                .ForeignKey("dbo.ApplicationUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.GroupId);
            
            AddColumn("dbo.ApplicationRoles", "Description", c => c.String(maxLength: 250));
            AddColumn("dbo.ApplicationRoles", "Discriminator", c => c.String(nullable: false, maxLength: 128));
            AlterColumn("dbo.ApplicationUserClaims", "Id", c => c.Int(nullable: false));
            AlterColumn("dbo.ApplicationUserClaims", "UserId", c => c.String(nullable: false, maxLength: 128));
            AddPrimaryKey("dbo.ApplicationUserClaims", "UserId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ApplicationUserGroups", "UserId", "dbo.ApplicationUsers");
            DropForeignKey("dbo.ApplicationUserGroups", "GroupId", "dbo.ApplicationGroups");
            DropForeignKey("dbo.ApplicationRoleGroups", "RoleId", "dbo.ApplicationRoles");
            DropForeignKey("dbo.ApplicationRoleGroups", "GroupId", "dbo.ApplicationGroups");
            DropIndex("dbo.ApplicationUserGroups", new[] { "GroupId" });
            DropIndex("dbo.ApplicationUserGroups", new[] { "UserId" });
            DropIndex("dbo.ApplicationRoleGroups", new[] { "RoleId" });
            DropIndex("dbo.ApplicationRoleGroups", new[] { "GroupId" });
            DropPrimaryKey("dbo.ApplicationUserClaims");
            AlterColumn("dbo.ApplicationUserClaims", "UserId", c => c.String());
            AlterColumn("dbo.ApplicationUserClaims", "Id", c => c.Int(nullable: false, identity: true));
            DropColumn("dbo.ApplicationRoles", "Discriminator");
            DropColumn("dbo.ApplicationRoles", "Description");
            DropTable("dbo.ApplicationUserGroups");
            DropTable("dbo.ApplicationRoleGroups");
            DropTable("dbo.ApplicationGroups");
            AddPrimaryKey("dbo.ApplicationUserClaims", "Id");
            RenameTable(name: "dbo.ApplicationRoles", newName: "IdentityRoles");
            RenameTable(name: "dbo.ApplicationUserRoles", newName: "IdentityUserRoles");
            RenameTable(name: "dbo.ApplicationUserLogins", newName: "IdentityUserLogins");
            RenameTable(name: "dbo.ApplicationUserClaims", newName: "IdentityUserClaims");
        }
    }
}
