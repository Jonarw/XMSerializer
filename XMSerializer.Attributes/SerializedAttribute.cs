using System;

namespace XmSerializer
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class SerializedAttribute : Attribute
    {
        public SerializedAttribute()
        {
            this.Alias = null;
        }

        public SerializedAttribute(string alias)
        {
            this.Alias = alias;
        }

        public string Alias { get; }

        public bool SkipInstantiating { get; set; }
        public bool Serialize { get; set; } = true;
        public bool Deserialize { get; set; } = true;
        public object DefaultValue { get; set; }
        public bool Obfuscate { get; set; }
        public string LegacyName { get; set; }
    }
}