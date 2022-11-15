using Corely.Connections.Proxies;
using Corely.Security.Authentication;

namespace Corely.Connections
{
    public class OdbcConn
    {
        #region Constructor

        /// <summary>
        /// Constructor with credentials
        /// </summary>
        /// <param name="_credential"></param>
        public OdbcConn(OdbcCredentials _credentials)
        {
            Credentials = _credentials;
            Proxy = new OdbcProxy();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Credentials for connecting
        /// </summary>
        public OdbcCredentials Credentials { get; internal set; }

        /// <summary>
        /// Proxy for database
        /// </summary>
        public OdbcProxy Proxy { get; set; }

        /// <summary>
        /// Proxy connection status
        /// </summary>
        public bool IsConnected => Proxy.IsConnected;

        #endregion

        #region Connection

        /// <summary>
        /// Connect to web client
        /// </summary>
        public void Connect()
        {
            Proxy.Connect(Credentials);
        }

        /// <summary>
        /// Disconnect web service connection
        /// </summary>
        public void Disconnect()
        {
            Proxy.Disconnect();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Get underlying database information
        /// </summary>
        /// <returns></returns>
        public string GetDbInfo()
        {
            return $"{nameof(Proxy.OdbcConnection.Driver)}: {Proxy.OdbcConnection.Driver};" +
                $"{nameof(Proxy.OdbcConnection.State)}: {Proxy.OdbcConnection.State};" +
                $"{nameof(Proxy.OdbcConnection.ServerVersion)}: {Proxy.OdbcConnection.ServerVersion};";
        }

        #endregion
    }
}
