namespace nCode.CMS.Models
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class RenameToContentPartInstance : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CMS_ContentPartInstance",
                c => new
                {
                    ID = c.Guid(nullable: false),
                    ContainerID = c.Guid(nullable: false),
                    DisplayIndex = c.Int(nullable: false),
                    ContentPartID = c.Guid(nullable: false),
                    RootContainerID = c.Guid(nullable: false)
                })
            .PrimaryKey(t => t.ID)
            .Index(t => new { t.ContainerID, t.DisplayIndex }, name: "IX_CMS_ContentPartInstance_ContainerID_DisplayIndex", unique: true);

            if (SqlUtilities.ColumnExist("CMS_ContentPart", "ParentContentPartID"))
            {
                Sql("INSERT INTO CMS_ContentPartInstance SELECT NEWID(), ISNULL(ParentContentPartID, ContentPageID), DisplayIndex, ID, ContentPageID FROM CMS_ContentPart");
                DropColumn("dbo.CMS_ContentPart", "ParentContentPartID");            
            }

            DropForeignKey("dbo.CMS_ContentPart", "FK_CMS_ContentPart_CMS_ContentPage");
            DropIndex("dbo.CMS_ContentPart", new[] { "ContentPageID" });
            CreateIndex("dbo.CMS_ContentPartInstance", "ContentPartID");
            AddForeignKey("dbo.CMS_ContentPartInstance", "ContentPartID", "dbo.CMS_ContentPart", "ID", cascadeDelete: true);
            DropColumn("dbo.CMS_ContentPart", "ContentPageID");
            DropColumn("dbo.CMS_ContentPart", "DisplayIndex");
        }

        public override void Down()
        {
            AddColumn("dbo.CMS_ContentPart", "DisplayIndex", c => c.Int(nullable: false));
            AddColumn("dbo.CMS_ContentPart", "ParentContentPartID", c => c.Guid());
            AddColumn("dbo.CMS_ContentPart", "ContentPageID", c => c.Guid(nullable: false));
            DropForeignKey("dbo.CMS_ContentPartInstance", "ContentPartID", "dbo.CMS_ContentPart");
            DropIndex("dbo.CMS_ContentPartInstance", new[] { "ContentPartID" });
            CreateIndex("dbo.CMS_ContentPart", "ContentPageID");
            AddForeignKey("CMS_ContentPart", "ContentPageID", "CMS_ContentPage", "ID", cascadeDelete: true, name: "FK_CMS_ContentPart_CMS_ContentPage");

            DropTable("dbo.CMS_ContentPartInstance");
        }
    }
}
