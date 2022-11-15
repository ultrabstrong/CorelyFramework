using System;

namespace UsefulUtilities.Security.Authentication
{
    [Serializable]
    public enum AuthenticationType
    {
        Basic = 0,
        Windows = 1,
        Token = 2
    }
}
