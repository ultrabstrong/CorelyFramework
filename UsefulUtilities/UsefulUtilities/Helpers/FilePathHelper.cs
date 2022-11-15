using System.IO;

namespace UsefulUtilities.Helpers
{
    public static class FilePathHelper
    {
        /// <summary>
        /// Build overwrite protected file path
        /// </summary>
        /// <param name="filepath"></param>
        /// <returns></returns>
        public static string GetOverwriteProtectedPath(string filepath)
        {
            // Create new file info for working with path
            FileInfo info = new FileInfo(filepath);
            // Return this path if path doesn't already exist
            if (!info.Exists) { return filepath; }
            // Find overwrite protected path
            FileInfo newinfo = null;
            int i = 0;
            do
            {
                newinfo = new FileInfo($"{info.DirectoryName}\\{info.Name.Replace(info.Extension, $"-[{++i}]")}{info.Extension}");
            }
            while (newinfo.Exists);
            // Return new file path
            return newinfo.FullName;
        }

        /// <summary>
        /// Get file name for file path
        /// </summary>
        /// <param name="filepath"></param>
        /// <returns></returns>
        public static string GetFileName(string filepath)
        {
            FileInfo info = new FileInfo(filepath);
            return info.Name;
        }

        /// <summary>
        /// Get file name for file path
        /// </summary>
        /// <param name="filepath"></param>
        /// <returns></returns>
        public static string GetFileNameNoExt(string filepath)
        {
            FileInfo info = new FileInfo(filepath);
            // Remove last occurrence of the extension
            int place = info.Name.LastIndexOf(info.Extension);
            if (place == -1) { return info.Name; }
            return info.Name.Remove(place, info.Extension.Length).Insert(place, "");
        }
    }
}
