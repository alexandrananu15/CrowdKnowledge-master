namespace CrowdKnowledge2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Articols",
                c => new
                    {
                        IDArticol = c.Int(nullable: false, identity: true),
                        TitluArticol = c.String(nullable: false, maxLength: 100),
                        ContinutArticol = c.String(nullable: false),
                        Data = c.DateTime(nullable: false),
                        IDDomeniu = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.IDArticol)
                .ForeignKey("dbo.Domenius", t => t.IDDomeniu, cascadeDelete: true)
                .Index(t => t.IDDomeniu);
            
            CreateTable(
                "dbo.Capitols",
                c => new
                    {
                        IDCapitol = c.Int(nullable: false, identity: true),
                        TitluCapitol = c.String(nullable: false, maxLength: 100),
                        Content = c.String(nullable: false),
                        Data = c.DateTime(nullable: false),
                        IDArticol = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.IDCapitol)
                .ForeignKey("dbo.Articols", t => t.IDArticol, cascadeDelete: true)
                .Index(t => t.IDArticol);
            
            CreateTable(
                "dbo.Domenius",
                c => new
                    {
                        IDDomeniu = c.Int(nullable: false, identity: true),
                        NumeDomeniu = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.IDDomeniu);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Articols", "IDDomeniu", "dbo.Domenius");
            DropForeignKey("dbo.Capitols", "IDArticol", "dbo.Articols");
            DropIndex("dbo.Capitols", new[] { "IDArticol" });
            DropIndex("dbo.Articols", new[] { "IDDomeniu" });
            DropTable("dbo.Domenius");
            DropTable("dbo.Capitols");
            DropTable("dbo.Articols");
        }
    }
}
