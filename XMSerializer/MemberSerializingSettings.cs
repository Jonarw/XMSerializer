using System;
using System.Collections.Generic;

namespace XmSerializer
{
    public class MemberSerializingSettings
    {
        public MemberSerializingSettings(string memberName, string alias, bool isProperty)
        {
            this.MemberName = memberName;
            this.Alias = alias;
            this.IsProperty = isProperty;
        }

        public MemberSerializingSettings(string memberName, bool isProperty = true)
        {
            this.MemberName = memberName;
            this.Alias = memberName;
            this.IsProperty = isProperty;
        }

        public bool IsProperty { get; }
        public string Alias { get; }
        public string MemberName { get; }
        public bool SkipInstantiating { get; set; }
        public object DefaultValue { get; set; }
        public Func<object> DefaultValueFunc { get; set; }
        public bool Obfuscate { get; set; }
        public IList<string> LegacyNames { get; set; } = new List<string>();
        public bool Serialize { get; set; } = true;
        public bool Deserialize { get; set; } = true;
    }
}