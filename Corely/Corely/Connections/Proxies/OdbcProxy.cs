using Corely.Security.Authentication;
using System;
using System.Data.Odbc;

namespace Corely.Connections.Proxies
{
    public class OdbcProxy : IDisposable
    {
        #region Constructor / Initialization

        /// <summary>
        /// Constructor for proxy
        /// </summary>
        public OdbcProxy() { }

        #endregion

        #region Properties

        /// <summary>
        /// ODBC conection for reading from / writing to database
        /// </summary>
        public OdbcConnection OdbcConnection { get; set; }

        /// <summary>
        /// Is there an ODBC connection
        /// </summary>
        public bool IsConnected { get; set; }

        #endregion

        #region Connection

        /// <summary>
        /// Connect to ODBC
        /// </summary>
        /// <param name="connectionstring"></param>
        public void Connect(OdbcCredentials credentials)
        {
            if (!IsConnected)
            {
                try
                {
                    // Connect to ODBC connection
                    string connectionstring = $"DSN={credentials.DSN};SERVER={credentials.Host};DATABASE={credentials.Database};UID={credentials.Username};PASSWORD={credentials.Password.DecryptedValue};";
                    if (credentials.Port != -1)
                    {
                        connectionstring += $";PORT={credentials.Port}";
                    }
                    OdbcConnection = new OdbcConnection(connectionstring);
                    OdbcConnection.Open();
                    IsConnected = true;
                }
                catch
                {
                    IsConnected = false;
                    throw;
                }
            }
        }

        /// <summary>
        /// Disconnect from ODBC
        /// </summary>
        public void Disconnect()
        {
            if (IsConnected)
            {
                // Disconnect from ODBC connection
                OdbcConnection?.Close();
                OdbcConnection?.Dispose();
                IsConnected = false;
            }
        }

        #endregion

        #region Dispose

        private bool _disposed = false;

        /// <summary>
        /// Dispose this object
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose this object only once
        /// </summary>
        /// <param name="disposing"></param>
        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    try { Disconnect(); }
                    catch { }
                }
                _disposed = true;
            }
        }

        #endregion
    }
}
