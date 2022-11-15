using Corely.Security.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Corely.FTP
{
    public static class FTPFactory
    {
        public static IFTPProxy CreateFTPProxy(GeneralCredentials credentials, string directory, bool enablessl)
        {
            return new FTPProxy(credentials)
            {
                Directory = directory,
                EnableSSL = enablessl
            };
        }

        public static IFTPProxy CreateSFTPProxy(GeneralCredentials credentials, string directory)
        {
            return new SFTPProxy(credentials)
            {
                Directory = directory
            };
        }
    }
}
