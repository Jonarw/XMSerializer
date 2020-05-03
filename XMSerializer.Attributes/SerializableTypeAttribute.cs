using System;

namespace XmSerializer
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Interface)]
    public sealed class SerializableTypeAttribute : Attribute
    {
        public SerializableTypeAttribute()
        {
            this.Alias = null;
        }

        public SerializableTypeAttribute(string alias)
        {
            this.Alias = alias;
        }

        public string Alias { get; }
        public bool SkipConstructor { get; set; } = true;
        public bool IncludeAncestors { get; set; } = true;
        public bool IsCollection { get; set; }
        public bool IsDictionary { get; set; }
        public MemberFilter ImplicitMemberFilter { get; set; }
        public string LegacyName { get; set; }
    }
}