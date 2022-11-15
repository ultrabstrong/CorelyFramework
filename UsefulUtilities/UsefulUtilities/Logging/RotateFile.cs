using System;

namespace UsefulUtilities.Logging
{
    [Flags]
    public enum RotateFile
    {
        None = 1,
        Daily = 2,
        Size = 4
    }
}
