namespace nCode.CMS.Model
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            
            /*
            CreateTable(
                "dbo.CMS_ContentPages",
                c => new
                    {
                        ID = c.Guid(nullable: false),
                        Created = c.DateTime(nullable: false),
                        Modified = c.DateTime(nullable: false),
                        Language = c.String(maxLength: 10),
                        Parent = c.Guid(nullable: false),
                        Index = c.Int(nullable: false),
                        StaticTitle = c.String(maxLength: 255),
                        StaticPath = c.String(maxLength: 255),
                        ContentType = c.Guid(nullable: false),
                        PageContent = c.String(),
                        LinkUrl = c.String(maxLength: 255),
                        LinkMode = c.Int(nullable: false),
                        IsProtected = c.Boolean(nullable: false),
                        ShowInMenu = c.Boolean(nullable: false),
                        ValidFrom = c.DateTime(),
                        ValidTo = c.DateTime(),
                        Title = c.String(nullable: false, maxLength: 255),
                        TitleMode = c.Int(nullable: false),
                        Keywords = c.String(maxLength: 255),
                        Description = c.String(maxLength: 255),
                        MasterPageFile = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => t.ID);
            */
            if (SqlUtilities.TableExist("CMS_ContentPartProperties"))
            {
                RenameTable("CMS_ContentPartProperties", "CMS_ContentPartProperty");
            }
            else if (!SqlUtilities.TableExist("CMS_ContentPartProperty"))
            {
                CreateTable(
                    "CMS_ContentPartProperty",
                    c => new
                        {
                            ID = c.Guid(nullable: false),
                            ContentPartID = c.Guid(nullable: false),
                            Key = c.String(nullable: false, maxLength: 255),
                            Value = c.String(),
                        })
                    .PrimaryKey(t => t.ID)
                    //.ForeignKey("CMS_ContentPart", t => t.ContentPartID)
                    .Index(t => t.ContentPartID)
                    .Index(t => new { t.ContentPartID, t.Key }, name: "IX_CMS_ContentPageProperty_ContentPage_Key", unique: true);

                AddForeignKey("CMS_ContentPartProperty", "ContentPartID", "CMS_ContentPart", "ID", cascadeDelete: true, name: "FK_CMS_ContentPartProperty_CMS_ContentPart");
            }

            if (SqlUtilities.TableExist("CMS_ContentParts"))
            {
                RenameTable("CMS_ContentParts", "CMS_ContentPart");
            }
            else if (!SqlUtilities.TableExist("CMS_ContentPart"))
            {
                CreateTable(
                    "CMS_ContentPart",
                    c => new
                        {
                            ID = c.Guid(nullable: false),
                            ContentPageID = c.Guid(nullable: false),
                            ParentContentPartID = c.Guid(),
                            DisplayIndex = c.Int(nullable: false),
                            ContentTypeID = c.Guid(nullable: false),
                        })
                    .PrimaryKey(t => t.ID)
                    //.ForeignKey("CMS_ContentPage", t => t.ContentPageID, cascadeDelete: true, name: "FK_CMS_ContentPart_CMS_ContentPage")
                    .Index(t => t.ContentPageID);

                AddForeignKey("CMS_ContentPart", "ContentPageID", "CMS_ContentPage", "ID", cascadeDelete: true, name: "FK_CMS_ContentPart_CMS_ContentPage");
            }
            
        }
        
        public override void Down()
        {
            DropForeignKey("CMS_ContentPartProperty", "ContentPartID", "CMS_ContentPart");
            DropForeignKey("CMS_ContentPart", "ContentPageID", "CMS_ContentPage");
            DropIndex("CMS_ContentPartProperty", new[] { "ContentPartID" });
            DropIndex("CMS_ContentPart", new[] { "ContentPageID" });
            DropIndex("CMS_ContentPartProperty", new[] { "ContentPartID", "t.Key" });
            DropTable("CMS_ContentPart");
            DropTable("CMS_ContentPartProperty");
            //DropTable("dbo.CMS_ContentPages");
        }
    }
}
