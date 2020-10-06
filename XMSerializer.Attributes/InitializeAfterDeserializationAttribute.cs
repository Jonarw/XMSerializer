using System;

namespace XmSerializer
{
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class InitializeAfterDeserializationAttribute : Attribute
    {
        public InitializeAfterDeserializationAttribute(Type type)
        {
            this.InitializationType = type;
        }

        public InitializeAfterDeserializationAttribute()
        {
            this.InitializationType = null;
        }

        public Type InitializationType { get; }
    }
}