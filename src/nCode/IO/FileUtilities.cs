using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Web;

namespace nCode.IO
{
    public static class FileUtilities
    {
        /// <summary>
        /// Returns a path that is safe for naming resources that should be available over the Http protocol.
        /// </summary>
        public static string SafePath(string path)
        {
            path = path.Replace("\\", "/");

            path = path.Replace(" ", "-");
            while (path.IndexOf("--") != -1)
                path = path.Replace("--", "-");

            path = path.Replace("æ", "ae");
            path = path.Replace("ø", "oe");
            path = path.Replace("å", "aa");
            path = path.Replace("Æ", "Ae");
            path = path.Replace("Ø", "Oe");
            path = path.Replace("Å", "Aa");

            path = Regex.Replace(path, @"[^A-Za-z0-9_\-\./]", string.Empty);

            return path;
        }

        /// <summary>
        /// Returns a name of a given path appended with a hythen and a number, such that the path does not exists.
        /// </summary>
        /// <param name="physicalFilePath"></param>
        /// <returns></returns>
        public static string ResolveNameConflict(string physicalFilePath)
        {
            if (!File.Exists(physicalFilePath) && !Directory.Exists(physicalFilePath))
                return physicalFilePath;

            string name = Path.GetDirectoryName(physicalFilePath) + "\\" + Path.GetFileNameWithoutExtension(physicalFilePath);
            string ext = Path.GetExtension(physicalFilePath);
            int i = 1;
            do
            {
                physicalFilePath = name + "-" + i + ext;
                i++;
            } while (File.Exists(physicalFilePath) || Directory.Exists(physicalFilePath));

            return physicalFilePath;
        }

        public static string GetVirtualPath(string physicalPath)
        {
            if (physicalPath == null)
                throw new ArgumentNullException("physicalPath");

            var applicationPath = HttpContext.Current.Server.MapPath("~/");

            if (!physicalPath.StartsWith(applicationPath))
                throw new ArgumentException(string.Format("Cannot convert the physical path '{0}' to a virtual path since it is not a subpath of the current application.", physicalPath));

            var virtualPath = physicalPath.Substring(applicationPath.Length).Replace("\\", "/");

            if (!virtualPath.StartsWith("/"))
                virtualPath = "/" + virtualPath;

            return virtualPath;
        }
    }
}
