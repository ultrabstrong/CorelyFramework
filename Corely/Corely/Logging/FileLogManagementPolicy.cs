using System;
using System.IO;
using rm = Corely.Resources.Logging.LogDeletionPolicy;

namespace Corely.Logging
{
    [Serializable]
    public class FileLogManagementPolicy
    {
        #region Constructor

        /// <summary>
        /// Constructor with parent object
        /// </summary>
        /// <param name="parent"></param>
        public FileLogManagementPolicy(FileLogger parent)
        {
            if (parent == null)
            {
                throw new Exception(rm.parentParamNull);
            }
            Parent = parent;
        }

        #endregion

        #region Properties

        /// <summary>
        /// File logger that this policy applies to
        /// </summary>
        public FileLogger Parent { get; internal set; }

        /// <summary>
        /// How often to run deletion
        /// </summary>
        public RunDeletion RunDeletion { get; set; } = RunDeletion.Daily;

        /// <summary>
        /// Last time the deletion policy was run
        /// </summary>
        public DateTime LastDeletionRun { get; set; } = DateTime.MinValue;

        /// <summary>
        /// Rotation settings for file
        /// </summary>
        public RotateFile RotateFile { get; set; } = RotateFile.Daily | RotateFile.Size;

        /// <summary>
        /// Days to wait before deleting files
        /// </summary>
        public int DeleteDaysOld { get; set; } = 5;

        /// <summary>
        /// KB size to exceed before deleting files
        /// </summary>
        public int RotateKBSize { get; set; } = 40000; // Default 40 MB

        #endregion

        #region Methods

        /// <summary>
        /// Get rotated log file path with all the rotation settings
        /// </summary>
        /// <returns></returns>
        public string GetRotatedLogFilePath()
        {
            // Build log file path
            string filepath = $"{Parent.LogDirectory}/{Parent.LogFile}";
            // Do not rotate if no rotation is set
            if (!RotateFile.HasFlag(RotateFile.None))
            {
                // Set rotate daily path
                if (RotateFile.HasFlag(RotateFile.Daily))
                {
                    filepath += $"{DateTime.Now.Month}-{DateTime.Now.Day}-{DateTime.Now.Year}";
                }
                // Set rotate size path
                if (RotateFile.HasFlag(RotateFile.Size))
                {
                    int i = 0;
                    int kbsize = 0;
                    do
                    {
                        // Create file info for path to test file size
                        string testpath = $"{filepath}_{++i}{Parent.LogExtension}";
                        FileInfo testinfo = new FileInfo(testpath);
                        // Only get size if file exists and is bigger than 1KB
                        if (testinfo.Exists && testinfo.Length > 1024)
                        {
                            kbsize = Convert.ToInt32(Math.Round(testinfo.Length / 1024.0));
                        }
                        else
                        {
                            kbsize = 0;
                        }
                    }
                    // Get next file if this file has already hit max capacity
                    while (kbsize > RotateKBSize);
                    // Set file path with current file rotation number
                    filepath += $"_{i}";
                }
            }
            // Add extension to file path
            filepath = $"{filepath}{Parent.LogExtension}";
            // Return complete file path
            return filepath;
        }

        /// <summary>
        /// Run deletion policy for files in directory
        /// </summary>
        public void RunDeletionPolicy()
        {
            // Only run if deletion policy is turned on and it was not already run today
            if (RunDeletion == RunDeletion.Daily && LastDeletionRun.Date != DateTime.Now.Date)
            {
                // Iterate log files in log directory
                foreach (string filepath in Directory.EnumerateFiles(Parent.LogDirectory, $"*{Parent.LogExtension}", SearchOption.TopDirectoryOnly))
                {
                    FileInfo info = new FileInfo(filepath);
                    // Delete file if it is older than the desired date
                    if (info.CreationTime.Date < (DateTime.Now.AddDays(-1 * DeleteDaysOld)).Date)
                    {
                        info.Delete();
                    }
                }
                // Set last run date so deletion policy doesn't run again today
                LastDeletionRun = DateTime.Now;
            }
        }

        #endregion

    }
}
