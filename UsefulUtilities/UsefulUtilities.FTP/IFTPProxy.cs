using UsefulUtilities.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UsefulUtilities.FTP
{
    public interface IFTPProxy
    {
        FTPResponse Connect(ILogger Logger = null);

        Task<FTPResponse> ConnectAsync(ILogger logger = null);

        FTPResponse GetFilesInDirectory(string directorypath, ILogger logger = null);

        Task<FTPResponse> GetFilesInDirectoryAsync(string directorypath, ILogger logger = null);

        FTPResponse GetFileContents(string filepath, ILogger logger = null);

        Task<FTPResponse> GetFileContentsAsync(string filepath, ILogger logger = null);

        FTPResponse CheckFileOrDirectoryExistence(string path, ILogger logger = null);

        Task<FTPResponse> CheckFileOrDirectoryExistenceAsync(string path, ILogger logger = null);

        FTPResponse GetOverwriteProtectedFilename(string filename, ILogger logger = null);

        Task<FTPResponse> GetOverwriteProtectedFilenameAsync(string filename, ILogger logger = null);

        FTPResponse UploadFile(string docpath, string filename, ILogger logger = null);

        Task<FTPResponse> UploadFileAsync(string docpath, string filename, ILogger logger = null);

        FTPResponse UploadFile(Stream docstream, string filename, ILogger logger = null);

        Task<FTPResponse> UploadFileAsync(Stream docstream, string filename, ILogger logger = null);

        FTPResponse WriteAllText(string text, string filename, bool append, ILogger logger = null);

        Task<FTPResponse> WriteAllTextAsync(string filename, string text, bool append, ILogger logger = null);

        FTPResponse DeleteIncompleteDocument(string filename, ILogger logger = null);

        Task<FTPResponse> DeleteIncompleteDocumentAsync(string filename, ILogger logger = null);

        void Disconnect();
    }
}
