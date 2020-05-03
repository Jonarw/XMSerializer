using System;

namespace XmSerializer
{
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class OnDeserializedAttribute : Attribute
    {
        public int Priority { get; }

        public OnDeserializedAttribute(int priority = 0)
        {
            this.Priority = priority;
        }
    }
}