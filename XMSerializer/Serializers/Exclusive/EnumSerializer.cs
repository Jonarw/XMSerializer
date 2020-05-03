using System;
using System.Xml.Linq;

namespace XmSerializer.Serializers
{
    public class EnumSerializer : ExclusiveSerializer
    {
        public const string EnumTag = "enum";
        public const string ValueTag = "value";
        public const string TypeTag = "type";
        public const string StringTag = "name";

        public EnumSerializer(XmSerializerModel serializerModel) : base(serializerModel)
        {
            serializerModel.AddType(new TypeSerializingSettings(typeof(Enum)));
        }

        public override bool CanDeserialize(string xmlName)
        {
            return xmlName.Equals(EnumTag);
        }

        public override bool CanSerialize(object instance, Type type)
        {
            return typeof(Enum).IsAssignableFrom(type);
        }

        public override object Deserialize(XElement xml)
        {
            var desc = (string)xml.Attribute(StringTag);
            var typeAlias = (string)xml.Attribute(TypeTag);
            var enumType = this.SerializerModel.GetTypeSettings(typeAlias).Type;
            Enum ret;
            try
            {
                ret = (Enum)Enum.Parse(enumType, desc);
            }
            catch (ArgumentException)
            {
                var val = (long)xml.Attribute(ValueTag);
                ret = (Enum)Enum.ToObject(this.SerializerModel.GetTypeSettings(typeAlias).Type, val);
            }

            return ret;
        }

        public override XElement Serialize(object instance, Type type)
        {
            var enu = (Enum)instance;
            var ret = new XElement(EnumTag);
            var val = Convert.ChangeType(enu, TypeCode.Int64);
            ret.SetAttributeValue(ValueTag, val);
            ret.SetAttributeValue(TypeTag, this.SerializerModel.RegisteredTypes[enu.GetType()].Alias);
            ret.SetAttributeValue(StringTag, enu.ToString());
            return ret;
        }
    }
}