using System;
using System.Xml.Linq;

namespace XmSerializer.Serializers
{
    public class NullSerializer : ExclusiveSerializer
    {
        public const string NullTag = "null";

        public NullSerializer(XmSerializerModel serializerModel) : base(serializerModel)
        {
        }

        public override bool CanDeserialize(string xmlName)
        {
            return xmlName.Equals(NullTag);
        }

        public override bool CanSerialize(object instance, Type type)
        {
            return instance == null;
        }

        public override object Deserialize(XElement xml)
        {
            return null;
        }

        public override XElement Serialize(object instance, Type type)
        {
            return new XElement(NullTag);
        }
    }
}