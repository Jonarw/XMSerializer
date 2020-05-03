using System;
using System.Linq;
using System.Xml.Linq;

namespace XmSerializer.Serializers
{
    public class NullableSerializer : ExclusiveSerializer
    {
        public const string NullableTag = "nullable";
        public const string TypeTag = "type";

        public NullableSerializer(XmSerializerModel serializerModel) : base(serializerModel)
        {
            serializerModel.AddType(new TypeSerializingSettings(typeof(Nullable<>)));
        }

        public override bool CanDeserialize(string xmlName)
        {
            return xmlName.Equals(NullableTag);
        }

        public override bool CanSerialize(object instance, Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }

        public override object Deserialize(XElement xml)
        {
            var typeString = (string)xml.Attribute(TypeTag);
            var type = this.SerializerModel.ParseTypeAlias(typeString);
            var nullableType = typeof(Nullable<>).MakeGenericType(type);
            var content = xml.Elements().First();

            var value = this.SerializerModel.DeserializeSubObject(content);

            return Activator.CreateInstance(nullableType, value);
        }

        public override XElement Serialize(object instance, Type type)
        {
            var typeString = this.SerializerModel.GetTypeAlias(type.GenericTypeArguments[0]);
            var ret = new XElement(NullableTag);
            ret.SetAttributeValue(TypeTag, typeString);

            var value = instance.GetPropertyValue<bool>("HasValue")
                ? instance.GetPropertyValue<object>("Value")
                : null;

            var content = this.SerializerModel.SerializeSubObject(value);
            ret.Add(content);
            return ret;
        }
    }
}