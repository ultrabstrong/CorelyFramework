using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Renci.SshNet;
using UsefulUtilities.Security.Authentication;
using UsefulUtilities.Services;
using UsefulUtilities.Logging;
using rm = UsefulUtilities.FTP.Resources.SFTPConnection;
using System.IO;
using System.Threading;
using Renci.SshNet.Sftp;

namespace UsefulUtilities.FTP
{
    [Serializable]
    public class SFTPProxy : IFTPProxy, IDisposable
    {
        #region Constructor / Initialization

        /// <summary>
        /// Default constructor
        /// </summary>
        public SFTPProxy() { }

        /// <summary>
        /// Parameterized constructor for easy initialization
        /// </summary>
        /// <param name="credentials"></param>
        public SFTPProxy(GeneralCredentials credentials)
        {
            Credentials = credentials;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Credentials for connecting to SFTP site
        /// </summary>
        public GeneralCredentials Credentials { get; set; }

        /// <summary>
        /// Root directory
        /// </summary>
        public string Directory
        {
            get { return _defaultDirectory; }
            set
            {
                _defaultDirectory = value;
                if (_defaultDirectory != null && !_defaultDirectory.EndsWith("/") && !_defaultDirectory.EndsWith("\\"))
                {
                    _defaultDirectory += "/";
                }
            }
        }
        private string _defaultDirectory = "/";

        /// <summary>
        /// Underlying SFTP connection
        /// </summary>
        public SftpClient Connection { get; internal set; }

        /// <summary>
        /// Is underlying connection connected
        /// </summary>
        public bool IsConnected { get; internal set; }

        #endregion

        #region Methods

        /// <summary>
        /// Create Connection
        /// </summary>
        /// <returns></returns>
        public FTPResponse Connect(ILogger Logger = null)
        {
            try { return ConnectAsync(Logger).GetAwaiter().GetResult(); } catch (Exception ex) { throw ex; }
        }

        /// <summary>
        /// Create connection asynchronously
        /// </summary>
        /// <returns></returns>
        public async Task<FTPResponse> ConnectAsync(ILogger logger = null)
        {
            return await Task.Factory.StartNew(() =>
            {
                FTPResponse response = new FTPResponse();
                try
                {
                    // Throw exception if credentials are not set
                    if (Credentials == null) { throw new Exception(rm.credentialsNotSet); }
                    // Create connection
                    ConnectionInfo info = null;
                    if (Credentials.Port > 0)
                    {
                        info = new ConnectionInfo(Credentials.Host, Credentials.Port, Credentials.Username, new AuthenticationMethod[] { new PasswordAuthenticationMethod(Credentials.Username, Credentials.Password.DecryptedValue) });
                    }
                    else
                    {
                        info = new ConnectionInfo(Credentials.Host, Credentials.Port, Credentials.Username, new AuthenticationMethod[] { new PasswordAuthenticationMethod(Credentials.Username, Credentials.Password.DecryptedValue) });
                    }
                    Connection = new SftpClient(info);
                    // Test connection by moving to root directory
                    Connection.Connect();
                    Connection.ChangeDirectory(Directory);
                    IsConnected = true;
                }
                catch (Exception ex)
                {
                    response.SetWithException(rm.connectionFailed, ex, System.Net.HttpStatusCode.InternalServerError);
                    logger?.WriteLog(rm.connectionFailed, ex, LogLevel.ERROR);
                }
                return response;
            }, TaskCreationOptions.LongRunning);
        }

        /// <summary>
        /// Get file names in directory
        /// </summary>
        /// <param name="directorypath"></param>
        /// <param name="logger"></param>
        /// <returns></returns>
        public FTPResponse GetFilesInDirectory(string directorypath, ILogger logger = null)
        {
            try { return GetFilesInDirectoryAsync(directorypath, logger).GetAwaiter().GetResult(); } catch (Exception ex) { throw ex; }
        }

        /// <summary>
        /// Get file names in directory async
        /// </summary>
        /// <param name="directorypath"></param>
        /// <param name="logger"></param>
        /// <returns></returns>
        public async Task<FTPResponse> GetFilesInDirectoryAsync(string directorypath, ILogger logger = null)
        {
            return await Task.Factory.StartNew(() =>
            {
                // Create return variable
                FTPResponse response = new FTPResponse();
                try
                {
                    if (!IsConnected) { throw new Exception(rm.proxyNotConnected); }
                    // Make sure directory exists
                    response = CheckFileOrDirectoryExistence(directorypath, logger);
                    if (response.Status != 200) { throw new Exception($"{response.Status} : {response.Message}", response.Exception); }
                    // Get file names from directory
                    List<SftpFile> files = Connection.ListDirectory($"{Directory}{directorypath}").ToList();
                    List<string> filenames = files.Where(m => m.IsDirectory == false).Select(m => m.Name).ToList();
                    response.Data = string.Join(Environment.NewLine, filenames);
                }
                catch (Exception ex)
                {
                    response.SetWithException(rm.operationFailed, ex, System.Net.HttpStatusCode.InternalServerError);
                    logger?.WriteLog(rm.operationFailed, ex, LogLevel.ERROR);
                }
                return response;
            }, TaskCreationOptions.LongRunning);
        }

        /// <summary>
        /// Get file contents
        /// </summary>
        /// <param name="filepath"></param>
        /// <param name="logger"></param>
        /// <returns></returns>
        public FTPResponse GetFileContents(string filepath, ILogger logger = null)
        {
            try { return GetFileContentsAsync(filepath, logger).GetAwaiter().GetResult(); } catch (Exception ex) { throw ex; }
        }

        /// <summary>
        /// Get file contents async
        /// </summary>
        /// <param name="filepath"></param>
        /// <param name="logger"></param>
        /// <returns></returns>
        public async Task<FTPResponse> GetFileContentsAsync(string filepath, ILogger logger = null)
        {
            return await Task.Factory.StartNew(() =>
            {
                // Create return variable
                FTPResponse response = new FTPResponse();
                try
                {
                    if (!IsConnected) { throw new Exception(rm.proxyNotConnected); }
                    // Make sure directory exists
                    response = CheckFileOrDirectoryExistence(filepath, logger);
                    if (response.Status != 200) { throw new Exception($"{response.Status} : {response.Message}", response.Exception); }
                    // Get file names from directory
                    using (Stream stream = new MemoryStream())
                    {
                        Connection.DownloadFile($"{Directory}{filepath}", stream);
                        stream.Position = 0;
                        using(StreamReader streamReader = new StreamReader(stream))
                        {
                            response.Data = streamReader.ReadToEnd();
                        }
                    }
                }
                catch (Exception ex)
                {
                    response.SetWithException(rm.operationFailed, ex, System.Net.HttpStatusCode.InternalServerError);
                    logger?.WriteLog(rm.operationFailed, ex, LogLevel.ERROR);
                }
                return response;
            }, TaskCreationOptions.LongRunning);
        }

        /// <summary>
        /// Check for file existence
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="logger"></param>
        /// <returns></returns>
        public FTPResponse CheckFileOrDirectoryExistence(string filename, ILogger logger = null)
        {
            try { return CheckFileOrDirectoryExistenceAsync(filename, logger).GetAwaiter().GetResult(); } catch (Exception ex) { throw ex; }
        }

        /// <summary>
        /// Check for file existence asynchronously
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="logger"></param>
        /// <returns></returns>
        public async Task<FTPResponse> CheckFileOrDirectoryExistenceAsync(string filename, ILogger logger = null)
        {
            return await Task.Factory.StartNew(() =>
            {
                // Create return variable
                FTPResponse response = new FTPResponse();
                try
                {
                    if (!IsConnected) { throw new Exception(rm.proxyNotConnected); }
                    bool exists = Connection.Exists($"{Directory}{filename}");
                    if(!exists)
                    {
                        response.SetWithStatus(rm.fileNotFound, System.Net.HttpStatusCode.NotFound);
                    }
                }
                catch (Exception ex)
                {
                    response.SetWithException(rm.operationFailed, ex, System.Net.HttpStatusCode.InternalServerError);
                    logger?.WriteLog(rm.operationFailed, ex, LogLevel.ERROR);
                }
                return response;
            }, TaskCreationOptions.LongRunning);
        }

        /// <summary>
        /// Get overwrite protected file name
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="extension"></param>
        /// <returns></returns>
        public FTPResponse GetOverwriteProtectedFilename(string filename, ILogger logger = null)
        {
            try { return GetOverwriteProtectedFilenameAsync(filename, logger).GetAwaiter().GetResult(); } catch (Exception ex) { throw ex; }
        }

        /// <summary>
        /// Check file name for uniqueness asynchronously
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="extension"></param>
        /// <returns></returns>
        public async Task<FTPResponse> GetOverwriteProtectedFilenameAsync(string filename, ILogger logger = null)
        {
            return await Task.Factory.StartNew(() =>
            {
                // Create return variable
                FTPResponse response = new FTPResponse();
                try
                {
                    if (!Connection.Exists($"{Directory}{filename}"))
                    {
                        // Return this file if it doesn't already exist
                        response.Data = filename;
                    }
                    else
                    {
                        // Find overwrite protected name
                        string newfile = null;
                        int i = 0;
                        do
                        {
                            newfile = filename.Insert(filename.LastIndexOf("."), $"-[{++i}]");
                        }
                        while (Connection.Exists($"{Directory}{newfile}"));
                        // Return new file path
                        response.Data = newfile;
                    }
                }
                catch (Exception ex)
                {
                    response.SetWithException(rm.connectionFailed, ex, System.Net.HttpStatusCode.InternalServerError);
                    logger?.WriteLog(rm.connectionFailed, ex, LogLevel.ERROR);
                }
                return response;
            }, TaskCreationOptions.LongRunning);
        }

        /// <summary>
        /// Upload a document
        /// </summary>
        /// <param name="docstream"></param>
        /// <param name="filename"></param>
        /// <returns></returns>
        public FTPResponse UploadFile(string docpath, string filename, ILogger logger = null)
        {
            try { return UploadFileAsync(docpath, filename, logger).GetAwaiter().GetResult(); } catch (Exception ex) { throw ex; }
        }

        /// <summary>
        /// Upload a document asynchronously
        /// </summary>
        /// <param name="docstream"></param>
        /// <param name="filename"></param>
        /// <returns></returns>
        public async Task<FTPResponse> UploadFileAsync(string docpath, string filename, ILogger logger = null)
        {
            return await Task.Factory.StartNew(() =>
            {
                FTPResponse response = new FTPResponse();
                try
                {
                    // Check if file exists
                    if (!File.Exists(docpath))
                    {
                        throw new Exception(rm.docpathInvalid);
                    }
                    // Get file stream
                    using(FileStream fs = new FileStream(docpath, FileMode.Open, FileAccess.Read))
                    {
                        response = UploadDocumentInternal(fs, filename, logger);
                    }
                }
                catch (Exception ex)
                {
                    response.SetWithException(rm.operationFailed, ex, System.Net.HttpStatusCode.InternalServerError);
                    logger?.WriteLog(rm.operationFailed, ex, LogLevel.ERROR);
                }
                return response;
            }, TaskCreationOptions.LongRunning);
        }

        /// <summary>
        /// Upload a document
        /// </summary>
        /// <param name="docstream"></param>
        /// <param name="filename"></param>
        /// <returns></returns>
        public FTPResponse UploadFile(Stream docstream, string filename, ILogger logger = null)
        {
            try { return UploadFileAsync(docstream, filename, logger).GetAwaiter().GetResult(); } catch (Exception ex) { throw ex; }
        }

        /// <summary>
        /// Upload a document asynchronously
        /// </summary>
        /// <param name="docstream"></param>
        /// <param name="filename"></param>
        /// <returns></returns>
        public async Task<FTPResponse> UploadFileAsync(Stream docstream, string filename, ILogger logger = null)
        {
            return await Task.Factory.StartNew(() =>
            {
                return UploadDocumentInternal(docstream, filename, logger);
            }, TaskCreationOptions.LongRunning);
        }

        /// <summary>
        /// Internal logic for uploading document
        /// </summary>
        /// <param name="docstream"></param>
        /// <param name="filename"></param>
        /// <param name="logger"></param>
        /// <returns></returns>
        private FTPResponse UploadDocumentInternal(Stream docstream, string filename, ILogger logger = null)
        {
            // Create return variable
            FTPResponse response = new FTPResponse();
            try
            {
                if (!IsConnected) { throw new Exception(rm.proxyNotConnected); }
                Connection.UploadFile(docstream, $"{Directory}{filename}");
            }
            catch (Exception ex)
            {
                response.SetWithException(rm.operationFailed, ex, System.Net.HttpStatusCode.InternalServerError);
                logger?.WriteLog(rm.operationFailed, ex, LogLevel.ERROR);
            }
            return response;
        }

        /// <summary>
        /// Write all text to a file
        /// </summary>
        /// <param name="text"></param>
        /// <param name="filename"></param>
        /// <param name="logger"></param>
        /// <returns></returns>
        public FTPResponse WriteAllText(string text, string filename, bool append, ILogger logger = null)
        {
            try { return WriteAllTextAsync(text, filename, append, logger).GetAwaiter().GetResult(); } catch (Exception ex) { throw ex; }
        }

        /// <summary>
        /// Write all text to a file asynchronously
        /// </summary>
        /// <param name="text"></param>
        /// <param name="filename"></param>
        /// <param name="logger"></param>
        /// <returns></returns>
        public async Task<FTPResponse> WriteAllTextAsync(string filename, string text, bool append, ILogger logger = null)
        {
            return await Task.Factory.StartNew(() =>
            {
                // Create return variable
                FTPResponse response = new FTPResponse();
                try
                {
                    if (!IsConnected) { throw new Exception(rm.proxyNotConnected); }
                    if (append)
                    {
                        Connection.AppendAllText($"{Directory}{filename}", text);
                    }
                    else
                    {
                        Connection.WriteAllText($"{Directory}{filename}", text);
                    }
                    
                }
                catch (Exception ex)
                {
                    response.SetWithException(rm.operationFailed, ex, System.Net.HttpStatusCode.InternalServerError);
                    logger?.WriteLog(rm.operationFailed, ex, LogLevel.ERROR);
                }
                return response;
            }, TaskCreationOptions.LongRunning);
        }

        /// <summary>
        /// Delete incomplete document uploads 
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public FTPResponse DeleteIncompleteDocument(string filename, ILogger logger = null)
        {
            try { return DeleteIncompleteDocumentAsync(filename).GetAwaiter().GetResult(); } catch (Exception ex) { throw ex; }
        }

        /// <summary>
        /// Delete incomplete document uploads asynchronously
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public async Task<FTPResponse> DeleteIncompleteDocumentAsync(string filename, ILogger logger = null)
        {
            return await Task.Factory.StartNew(() =>
            {
                // Create return variable
                FTPResponse response = new FTPResponse();
                try
                {
                    if (!IsConnected) { throw new Exception(rm.proxyNotConnected); }
                    Connection.Delete($"{Directory}{filename}");
                }
                catch (Exception ex)
                {
                    response.SetWithException(rm.connectionFailed, ex, System.Net.HttpStatusCode.InternalServerError);
                    logger?.WriteLog(rm.connectionFailed, ex, LogLevel.ERROR);
                }
                return response;
            }, TaskCreationOptions.LongRunning);
        }

        /// <summary>
        /// Dispose this object
        /// </summary>
        public void Disconnect()
        {
            Cleanup();
        }

        #endregion

        #region Dispose

        private bool _disposed = false;

        /// <summary>
        /// Dispose this
        /// </summary>
        public void Dispose()
        {
            Cleanup();
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Perform cleanup
        /// </summary>
        private void Cleanup()
        {
            if (_disposed) { return; }
            try
            {
                Connection?.Disconnect();
                Connection?.Dispose();
                Connection = null;
            }
            catch { }
            finally
            {
                IsConnected = false;
            }
        }

        #endregion
    }
}
