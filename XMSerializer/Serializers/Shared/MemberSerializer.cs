using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace XmSerializer.Serializers
{
    public class MemberSerializer : SharedSerializer
    {
        public const string PropertyTag = "property";
        public const string ObfuscatedTag = "obfuscatedproperty";
        public const string ContentTag = "content";
        public const string NameTag = "name";
        public const byte ObfuscateByte = 0xA4;

        public MemberSerializer(XmSerializerModel serializerModel) : base(serializerModel)
        {
        }

        public override bool CanSerialize(TypeSerializingSettings typeSettings)
        {
            return this.GetAllMembers(typeSettings).Any();
        }

        public override void Deserialize(object instance, XElement xml, Type type, TypeSerializingSettings typeSettings)
        {
            var processedMembers = new HashSet<MemberSerializingSettings>();
            var members = xml.Elements().Where(xe => xe.Name == PropertyTag || xe.Name == ObfuscatedTag).Select(
                xe =>
                {
                    if (xe.Name == PropertyTag)
                        return xe;

                    var obfuscatedString = (string)xe.Attribute(ContentTag);
                    var data = Convert.FromBase64String(obfuscatedString);
                    for (var i = 0; i < data.Length; i++)
                        data[i] = (byte)(data[i] ^ ObfuscateByte);

                    var contentString = Encoding.UTF8.GetString(data);
                    return XElement.Parse(contentString);
                });

            foreach (var memberXml in members)
            {
                var memberAlias = (string)memberXml.Attribute(NameTag);
                var memberSettings = this.GetMember(typeSettings, memberAlias);

                if (memberSettings?.Deserialize != true)
                    continue;

                processedMembers.Add(memberSettings);

                var objectXml = memberXml.Elements().FirstOrDefault();

                if (objectXml == null)
                    continue;

                if (!memberSettings.SkipInstantiating)
                {
                    var value = this.SerializerModel.DeserializeSubObject(objectXml);
                    if (!instance.TrySetMemberValue(memberSettings.MemberName, value, memberSettings.IsProperty))
                    {
                        bool success;
                        try
                        {
                            var memberType1 = memberSettings.IsProperty
                                ? type.GetInstanceProperty(memberSettings.MemberName).PropertyType
                                : type.GetInstanceField(memberSettings.MemberName).FieldType;
                            value = Convert.ChangeType(value, memberType1);
                            success = instance.TrySetMemberValue(memberSettings.MemberName, value, memberSettings.IsProperty);
                        }
                        catch (Exception)
                        {
                            success = false;
                        }

                        if (!success)
                            Console.WriteLine($"WARNING: Could not set value to member {memberSettings.MemberName} on type {type}.");
                    }

                    continue;
                }

                if (!instance.TryGetMemberValue<object>(memberSettings.MemberName, memberSettings.IsProperty, out var member))
                    continue;

                var memberType = member.GetType();
                var memberTypeSettings = this.SerializerModel.GetTypeSettings(memberType);

                foreach (var serializer in this.SerializerModel.SharedSerializers.Where(s => s.CanSerialize(memberTypeSettings)))
                    serializer.Deserialize(member, objectXml, memberType, memberTypeSettings);
            }

            foreach (var memberSetting in this.GetAllMembers(typeSettings).Where(ms => !processedMembers.Contains(ms)))
            {
                if (memberSetting.DefaultValue != null)
                    instance.TrySetMemberValue(memberSetting.MemberName, memberSetting.DefaultValue, memberSetting.IsProperty);
                else if (memberSetting.DefaultValueFunc != null)
                    instance.TrySetMemberValue(memberSetting.MemberName, memberSetting.DefaultValueFunc(), memberSetting.IsProperty);
            }
        }

        public override void Serialize(object instance, XElement xml, Type type, TypeSerializingSettings typeSettings)
        {
            foreach (var member in this.GetAllMembers(typeSettings).Where(m => m.Serialize).OrderBy(m => m.Alias))
            {
                if (!instance.TryGetMemberValue<object>(member.MemberName, member.IsProperty, out var value))
                    throw new Exception($"Member {member.Alias} not found on type {type}");

                var content = this.SerializerModel.SerializeSubObject(value);

                var memberXml = new XElement(PropertyTag);
                memberXml.SetAttributeValue(NameTag, member.Alias);
                memberXml.Add(content);
                
                if (member.Obfuscate)
                {
                    var contentString = memberXml.ToString();
                    var data = Encoding.UTF8.GetBytes(contentString);
                    for (var i = 0; i < data.Length; i++)
                        data[i] = (byte)(data[i] ^ ObfuscateByte);

                    var obfuscatedString = Convert.ToBase64String(data);

                    var obfuscatedXml = new XElement(ObfuscatedTag);
                    obfuscatedXml.SetAttributeValue(ContentTag, obfuscatedString);
                    xml.Add(obfuscatedXml);
                }
                else
                {
                    xml.Add(memberXml);
                }
            }
        }

        private IEnumerable<MemberSerializingSettings> GetAllMembers(TypeSerializingSettings typeSettings)
        {
            foreach (var member in typeSettings.RegisteredMembers.Values)
                yield return member;

            if (!typeSettings.IncludeAncestors)
                yield break;

            var ancestorMembers = typeSettings.Type.GetAncestors()
                .Select(ancestor => this.SerializerModel.TryGetTypeSettings(ancestor))
                .Where(ts => ts != null)
                .SelectMany(type => type.RegisteredMembers.Values);

            foreach (var member in ancestorMembers)
                yield return member;
        }

        private MemberSerializingSettings GetMember(TypeSerializingSettings typeSettings, string memberAlias)
        {
            MemberSerializingSettings ret;

            bool GetMemberFromType(TypeSerializingSettings typeSettingsInternal)
            {
                return typeSettingsInternal.RegisteredMembers.TryGetValue(memberAlias, out ret)
                       || typeSettingsInternal.RegisteredLegacyMembers.TryGetValue(memberAlias, out ret);
            }

            if (GetMemberFromType(typeSettings))
                return ret;

            if (!typeSettings.IncludeAncestors)
                return null;

            if (typeSettings.Type.GetAncestors()
                .Select(ancestor => this.SerializerModel.TryGetTypeSettings(ancestor))
                .Where(ts => ts != null)
                .Any(GetMemberFromType))
            {
                return ret;
            }

            return null;
        }
    }
}