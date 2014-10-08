using System;
using System.Data;

using nCode.UI;

namespace nCode.CMS
{
    /// <summary>
    /// Implements ways for converting content data records to objects.
    /// </summary>
    internal static class ContentPageFactory
    {
        public static string SelectFields
        {
            get {
                return "ID, " +
                "Created, " +
                "Modified, " +
                "Language, " +
                "ParentID, " +
                "DisplayIndex, " +
                "StaticTitle, " +
                "StaticPath, " +

                // Content
                "ContentType, " +
                "LinkUrl, " +
                "LinkMode, " +

                // Security and Access
                "IsProtected, " +
                "ShowInMenu, " +
                "ValidFrom, " +
                "ValidTo, " +

                // Look and Feel
                "Title, " +
                "TitleMode, " +
                "MasterPageFile, " +
                "Image1, " +
                "Image2, " +
                "Image3";
            }
        }

        private static ContentPage CreateContentPageInternal(IDataRecord rec, ContentPage content)
        {
            // Core Settings
            content.Created = (DateTime)rec["Created"];
            content.Modified = (DateTime)rec["Modified"];

            if (rec["ContentType"] != DBNull.Value)
                content.ContentType = CmsSettings.ContentTypes[(Guid)rec["ContentType"]];

            content.Language = SqlUtilities.ToString(rec["Language"]);
            content.ParentId = SqlUtilities.ToGuid(rec["ParentID"]);
            content.DisplayIndex = (int)rec["DisplayIndex"];
            content.StaticTitle = SqlUtilities.ToString(rec["StaticTitle"]);
            content.StaticPath = SqlUtilities.ToString(rec["StaticPath"]);

            // Content
            content.LinkUrl = SqlUtilities.ToString(rec["LinkUrl"]);
            content.LinkMode = (LinkMode)rec["LinkMode"];

            // Security and Access
            content.IsProtected = (bool)rec["IsProtected"];
            content.ShowInMenu = (bool)rec["ShowInMenu"];
            content.ValidFrom = SqlUtilities.ToDateTime(rec["ValidFrom"]);
            content.ValidTo = SqlUtilities.ToDateTime(rec["ValidTo"]);

            // Look and Feel
            content.Title = (string)rec["Title"];
            content.TitleMode = (TitleMode)rec["TitleMode"];
            content.MasterPageFile = SqlUtilities.ToString(rec["MasterPageFile"]);
            content.Image1 = SqlUtilities.ToString(rec["Image1"]);
            content.Image2 = SqlUtilities.ToString(rec["Image2"]);
            content.Image3 = SqlUtilities.ToString(rec["Image3"]);

            content.IsEditable = true;

            if (content.ContentType != null)
                content.ContentType.ContentPageCreated(content);

            return content;
        }

        /// <summary>
        /// Converts an Content data record to a <see cref="nCode.CMS.Content">Content</see> object.
        /// </summary>
        /// <param name="rec">The content data record.</param>
        /// <returns>The content object.</returns>
        public static ContentPage CreateContentPage(IDataRecord rec)
        {
            return CreateContentPageInternal(rec, new ContentPage((Guid)rec["ID"]));
        }

        public static ContentPageNode CreateContentPageNode(IDataRecord rec)
        {
            return (ContentPageNode)CreateContentPageInternal(rec, new ContentPageNode((Guid)rec["ID"]));
        }
    }
}
