using System;

namespace XmSerializer
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class NotSerializedAttribute : Attribute
    {
    }
}