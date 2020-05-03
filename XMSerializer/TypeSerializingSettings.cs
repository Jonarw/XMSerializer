using System;
using System.Collections.Generic;

namespace XmSerializer
{
    public class TypeSerializingSettings
    {
        private readonly Dictionary<string, MemberSerializingSettings> _RegisteredMembers = new Dictionary<string, MemberSerializingSettings>();
        private readonly Dictionary<string, MemberSerializingSettings> _RegisteredLegacyMembers = new Dictionary<string, MemberSerializingSettings>();

        public TypeSerializingSettings(string alias, Type type)
        {
            this.Alias = alias;
            this.Type = type;
        }

        public TypeSerializingSettings(Type type) : this(type.Name, type)
        {
        }

        public string Alias { get; }
        public IReadOnlyDictionary<string, MemberSerializingSettings> RegisteredMembers => this._RegisteredMembers;
        public IReadOnlyDictionary<string, MemberSerializingSettings> RegisteredLegacyMembers => this._RegisteredLegacyMembers;
        public bool IsCollection { get; set; }
        public bool IsDictionary { get; set; }
        public bool SkipConstructor { get; set; } = true;
        public bool IncludeAncestors { get; set; } = true;
        public IList<string> LegacyNames { get; } = new List<string>();
        public Type Type { get; }

        public IReadOnlyList<MemberSerializingSettings> Members
        {
            set
            {
                foreach (var settings in value)
                    this.AddMember(settings);
            }
        }

        public void AddMember(MemberSerializingSettings settings)
        {
            if (this.RegisteredMembers.ContainsKey(settings.Alias))
                throw new Exception($"Field {settings.Alias} already added to type {this.Alias}.");

            foreach (var legacyName in settings.LegacyNames)
                this._RegisteredLegacyMembers.Add(legacyName, settings);

            this._RegisteredMembers.Add(settings.Alias, settings);
        }
    }
}