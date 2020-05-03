using System;

namespace XmSerializer
{
    [Flags]
    public enum MemberFilter
    {
        Explicit = 0,
        NonPublicFields = 1,
        PublicFields = 2,
        NonPublicProperties = 4,
        PublicProperties = 8,
    }
}