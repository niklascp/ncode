namespace nCode.CMS.Models
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddingContentBlocks : DbMigration
    {
        public override void Up()
        {
            if (!SqlUtilities.TableExist("CMS_ContentBlock"))
            {
                CreateTable(
                    "dbo.CMS_ContentBlock",
                    c => new
                    {
                        ID = c.Guid(nullable: false),
                        Created = c.DateTime(nullable: false),
                        Modified = c.DateTime(nullable: false),
                        Code = c.String(nullable: false, maxLength: 255),
                    })
                    .PrimaryKey(t => t.ID);
            }

            if (!SqlUtilities.TableExist("CMS_ContentBlockLocalization"))
            {
                CreateTable(
                    "dbo.CMS_ContentBlockLocalization",
                    c => new
                        {
                            ID = c.Guid(nullable: false),
                            ContentBlockID = c.Guid(nullable: false),
                            Cultre = c.String(maxLength: 255),
                            Content = c.String(),
                        })
                    .PrimaryKey(t => t.ID)
                    .ForeignKey("dbo.CMS_ContentBlock", t => t.ContentBlockID, cascadeDelete: true)
                    .Index(t => t.ContentBlockID);
            }
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CMS_ContentBlockLocalization", "ContentBlockID", "dbo.CMS_ContentBlock");
            DropIndex("dbo.CMS_ContentBlockLocalization", new[] { "ContentBlockID" });
            DropTable("dbo.CMS_ContentBlockLocalization");
            DropTable("dbo.CMS_ContentBlock");
        }
    }
}
