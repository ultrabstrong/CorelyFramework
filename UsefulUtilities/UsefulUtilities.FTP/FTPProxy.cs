using UsefulUtilities.Logging;
using UsefulUtilities.Security.Authentication;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using rm = UsefulUtilities.FTP.Resources.FTPConnection;

namespace UsefulUtilities.FTP
{
    [Serializable]
    public class FTPProxy : IFTPProxy
    {
        #region Constructor / Initialization

        /// <summary>
        /// Default constructor
        /// </summary>
        public FTPProxy() { }

        /// <summary>
        /// Parameterized constructor for easy initialization
        /// </summary>
        /// <param name="credentials"></param>
        public FTPProxy(GeneralCredentials credentials)
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
        /// Enable SSL
        /// </summary>
        public bool EnableSSL { get; set; }

        /// <summary>
        /// Underlying SFTP connection
        /// </summary>
        public FtpWebRequest Connection { get; internal set; }

        /// <summary>
        /// Is underlying connection connected
        /// </summary>
        public bool IsConnected { get; internal set; }

        #endregion

        #region Methods

        /// <summary>
        /// Create Connection
        /// </summary>
        /// <param name="Logger"></param>
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

                    ChangeCertificateValidationCallback();
                    // Set user defined settings
                    Connection = (FtpWebRequest)WebRequest.Create(Credentials.Host + Directory);
                    if(!string.IsNullOrWhiteSpace(Credentials.Username))
                    {
                        Connection.Credentials = new NetworkCredential(Credentials.Username, Credentials.Password?.DecryptedValue ?? "");
                    }
                    Connection.EnableSsl = EnableSSL;
                    // Set default settings
                    Connection.Method = WebRequestMethods.Ftp.ListDirectory;
                    Connection.UsePassive = true;
                    Connection.UseBinary = true;
                    Connection.KeepAlive = false;
                    // Test connection
                    using (Connection.GetResponse()) ;
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
        /// Change certificate validation callback method
        /// </summary>
        public void ChangeCertificateValidationCallback()
        {
            // This statement is to ignore certification validation warning
            ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(OnValidateCertificate);
            ServicePointManager.Expect100Continue = true;
        }

        /// <summary>
        /// Automatically validate certificate
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="certificate"></param>
        /// <param name="chain"></param>
        /// <param name="sslPolicyErrors"></param>
        /// <returns></returns>
        private bool OnValidateCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) => true;

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
                    FtpWebRequest req = CreateNewRequest($"{Directory}{directorypath}", WebRequestMethods.Ftp.ListDirectory);
                    try
                    {
                        // Read file names from stream into response data
                        using (FtpWebResponse resp = (FtpWebResponse)req.GetResponse())
                        using (Stream stream = resp.GetResponseStream())
                        using (StreamReader streamReader = new StreamReader(stream))
                        {
                            response.Data = streamReader.ReadToEnd();
                        }
                    }
                    catch(Exception ex)
                    {
                        response.SetWithException(rm.failGetFiles, ex, System.Net.HttpStatusCode.InternalServerError);
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
                    FtpWebRequest req = CreateNewRequest($"{Directory}{filepath}", WebRequestMethods.Ftp.DownloadFile);
                    try
                    {
                        // Read file names from stream into response data
                        using (FtpWebResponse resp = (FtpWebResponse)req.GetResponse())
                        using (Stream stream = resp.GetResponseStream())
                        using (StreamReader streamReader = new StreamReader(stream))
                        {
                            response.Data = streamReader.ReadToEnd();
                        }
                    }
                    catch (Exception ex)
                    {
                        response.SetWithException(rm.failGetFiles, ex, System.Net.HttpStatusCode.InternalServerError);
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
                    bool exists = false;
                    FtpWebRequest req = CreateNewRequest($"{Directory}{filename}", WebRequestMethods.Ftp.GetDateTimestamp);
                    try 
                    { 
                        using (FtpWebResponse resp = (FtpWebResponse)req.GetResponse()) ;
                        exists = true;
                    } 
                    catch { }
                    if (!exists)
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
                    if (!IsConnected) { throw new Exception(rm.proxyNotConnected); }
                    // Check if original file name is unique
                    bool isunique = false;
                    FtpWebRequest req = CreateNewRequest(filename, WebRequestMethods.Ftp.GetDateTimestamp);
                    try { using (FtpWebResponse resp = (FtpWebResponse)req.GetResponse()) ; } catch { isunique = true; }
                    if (isunique)
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
                            // Check if modified file name is unique
                            newfile = filename.Insert(filename.LastIndexOf("."), $"-[{++i}]"); 
                            req = CreateNewRequest(newfile, WebRequestMethods.Ftp.GetDateTimestamp);
                            try { using (FtpWebResponse resp = (FtpWebResponse)req.GetResponse()) ; } catch { isunique = true; }
                        }
                        while (!isunique);
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
        public FTPResponse UploadFile(string filepath, string filename, ILogger logger = null)
        {
            try { return UploadFileAsync(filepath, filename, logger).GetAwaiter().GetResult(); } catch (Exception ex) { throw ex; }
        }

        /// <summary>
        /// Upload a document asynchronously
        /// </summary>
        /// <param name="docstream"></param>
        /// <param name="filename"></param>
        /// <returns></returns>
        public async Task<FTPResponse> UploadFileAsync(string filepath, string filename, ILogger logger = null)
        {
            return await Task.Factory.StartNew(() =>
            {
                FTPResponse response = new FTPResponse();
                try
                {
                    // Check if file exists
                    if (!File.Exists(filepath))
                    {
                        throw new Exception(rm.docpathInvalid);
                    }
                    // Get file stream
                    using (FileStream fs = new FileStream(filepath, FileMode.Open, FileAccess.Read))
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
                // Set up for uploading document stream
                byte[] buffer = new byte[4097];
                int bytes = 0, totalBytes = (int)docstream.Length;
                FtpWebRequest req = CreateNewRequest($"{Directory}{filename}", WebRequestMethods.Ftp.UploadFile);
                // Upload document 4097 bytes at a time
                using (docstream)
                using (Stream rs = req.GetRequestStream())
                {
                    while (totalBytes > 0)
                    {
                        // Upload next chuck of bytes
                        bytes = docstream.Read(buffer, 0, buffer.Length);
                        rs.Write(buffer, 0, bytes);
                        totalBytes -= bytes;
                    }
                    rs.Close();
                }
                // Set response data
                using (FtpWebResponse ftpresponse = (FtpWebResponse)req.GetResponse())
                {
                    response.Status = (int)ftpresponse.StatusCode;
                    response.Message = ftpresponse.StatusDescription;
                    ftpresponse.Close();
                }
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
                    // Set up for uploading document stream
                    byte[] buffer = new byte[4097];
                    byte[] textbyte = Encoding.ASCII.GetBytes(text);
                    int bytes = 0, totalBytes = textbyte.Length;
                    FtpWebRequest req = CreateNewRequest($"{Directory}{filename}", append ? WebRequestMethods.Ftp.AppendFile : WebRequestMethods.Ftp.UploadFile);
                   // Upload document 4097 bytes at a time
                    using (MemoryStream ms = new MemoryStream(textbyte))
                    using (Stream rs = req.GetRequestStream())
                    {
                        while (totalBytes > 0)
                        {
                            // Upload next chuck of bytes
                            bytes = ms.Read(buffer, 0, buffer.Length);
                            rs.Write(buffer, 0, bytes);
                            totalBytes -= bytes;
                        }
                        rs.Close();
                    }
                    // Set response data
                    using (FtpWebResponse ftpresponse = (FtpWebResponse)req.GetResponse())
                    {
                        response.Status = (int)ftpresponse.StatusCode;
                        response.Message = ftpresponse.StatusDescription;
                        ftpresponse.Close();
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
                    FtpWebRequest req = CreateNewRequest($"{Directory}{filename}", WebRequestMethods.Ftp.GetDateTimestamp);
                    // Try get response for file
                    FtpWebResponse ftpresponse = null;
                    try { ftpresponse = (FtpWebResponse)req.GetResponse(); } catch { ftpresponse = null; }
                    // Delete file if file exists
                    if (ftpresponse != null)
                    {
                        req = CreateNewRequest($"{Directory}{filename}", WebRequestMethods.Ftp.DeleteFile);
                        ftpresponse = (FtpWebResponse)req.GetResponse();
                    }
                    // Set response data
                    if (ftpresponse != null)
                    {
                        response.Status = (int)ftpresponse.StatusCode;
                        response.Message = ftpresponse.StatusDescription;
                        ftpresponse.Close();
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
        /// Create a new connection based on the original
        /// <para>Connection needs to be recreated for checking if file exists</para>
        /// </summary>
        private FtpWebRequest CreateNewRequest(string folderpath, string method)
        {
            FtpWebRequest request = null;
            try
            {
                if (Connection != null)
                {
                    request = (FtpWebRequest)WebRequest.Create($@"{Connection.RequestUri}/{folderpath}");
                    request.Credentials = Connection.Credentials;
                    request.EnableSsl = Connection.EnableSsl;
                    request.Method = method;
                    request.UseBinary = Connection.UseBinary;
                    request.UsePassive = Connection.UsePassive;
                    request.KeepAlive = Connection.KeepAlive;
                }
            }
            catch (Exception ex) { throw ex; }
            return request;
        }

        /// <summary>
        /// Dispose this object
        /// </summary>
        public void Disconnect()
        {
            
        }

        #endregion
    }
}
