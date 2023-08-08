using System;

namespace CorelyFramework.Logging
{
    [Flags]
    public enum RotateFile
    {
        None = 1,
        Daily = 2,
        Size = 4
    }
}
