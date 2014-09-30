namespace nCode
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Metadata_Initial : DbMigration
    {
        public override void Up()
        {
            if (SqlUtilities.TableExist("System_MetadataDescriptors"))
            {
                RenameIndex("System_MetadataDescriptors", "IX_System_MetadataDescriptors_ObjectTypeID_Name", "IX_ObjectTypeID_Name");
                RenameTable("System_MetadataDescriptors", "System_MetadataDescriptor");
            }
            else if (!SqlUtilities.TableExist("System_MetadataDescriptor"))
            {
                CreateTable(
                    "System_MetadataDescriptor",
                    c => new
                        {
                            ID = c.Guid(nullable: false),
                            ObjectTypeID = c.Guid(nullable: false),
                            DisplayIndex = c.Int(nullable: false),
                            Name = c.String(nullable: false, maxLength: 255),
                            DisplayText = c.String(nullable: false, maxLength: 255),
                            DisplayMode = c.Int(nullable: false),
                            EditControlPath = c.String(nullable: false, maxLength: 255),
                            EditControlArguments = c.String(maxLength: 255),
                        })
                    .PrimaryKey(t => t.ID)
                    .Index(t => new { t.ObjectTypeID, t.Name }, unique: true, name: "IX_ObjectTypeID_Name");
            }

            if (SqlUtilities.TableExist("System_MetadataProperties"))
            {
                RenameIndex("System_MetadataProperties", "IX_System_MetadataProperties_ObjectTypeID_ObjectID_Key", "IX_ObjectTypeID_ObjectID_Key");
                RenameTable("System_MetadataProperties", "System_MetadataProperty");
            }
            else if (!SqlUtilities.TableExist("System_MetadataProperty"))
            {
                CreateTable(
                    "System_MetadataProperty",
                    c => new
                        {
                            ID = c.Guid(nullable: false),
                            ObjectTypeID = c.Guid(nullable: false),
                            ObjectID = c.Guid(nullable: false),
                            Key = c.String(nullable: false, maxLength: 255),
                            Value = c.String(nullable: false),
                        })
                    .PrimaryKey(t => t.ID)
                    .Index(t => new { t.ObjectTypeID, t.ObjectID, t.Key }, unique: true, name: "IX_ObjectTypeID_ObjectID_Key");
            }
        }
        
        public override void Down()
        {
            DropIndex("System_MetadataProperty", "IX_ObjectTypeID_ObjectID_Key");
            DropIndex("System_MetadataDescriptor", "IX_ObjectTypeID_Name");
            DropTable("System_MetadataProperty");
            DropTable("System_MetadataDescriptor");
        }
    }
}
