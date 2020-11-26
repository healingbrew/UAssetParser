using System;

namespace UObjectDeserializer
{
    [Flags]
    public enum ErrorCodes
    {
        Success      = 0,
        Crash        = 1 << 0,
        FlagError    = 1 << 1,
        NotSupported = 1 << 2,
    }
}
