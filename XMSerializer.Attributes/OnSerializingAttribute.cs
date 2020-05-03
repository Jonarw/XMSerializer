using System;

namespace XmSerializer
{
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class OnSerializingAttribute : Attribute
    {
    }
}