using System;

namespace UsefulUtilities.Sage300HH2.Connection
{
    [Serializable]
    public class AuthenticateSessionResponse
    {
        public AuthenticatedClient AuthenticatedClient { get; set; }

        public AuthenticatedUser AuthenticatedUser { get; set; }

        public AuthenticationResult Result { get; set; }
    }
}
