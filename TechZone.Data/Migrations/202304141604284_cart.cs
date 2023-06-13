namespace TechZone.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class cart : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Orders", "CustomerID", c => c.String(maxLength: 128));
            CreateIndex("dbo.Orders", "CustomerID");
            AddForeignKey("dbo.Orders", "CustomerID", "dbo.ApplicationUsers", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Orders", "CustomerID", "dbo.ApplicationUsers");
            DropIndex("dbo.Orders", new[] { "CustomerID" });
            DropColumn("dbo.Orders", "CustomerID");
        }
    }
}
