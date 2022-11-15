using System;

namespace UsefulUtilities.Sage300HH2.Exceptions
{
    public class SageHH2NotConnectedException : Exception
    {
        public SageHH2NotConnectedException() : base() { }

        public SageHH2NotConnectedException(string message) : base(message) { }
    }
}
