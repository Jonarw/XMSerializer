using System;
using System.Collections;
using System.Linq;
using System.Xml.Linq;

namespace XmSerializer.Serializers
{
    public class ArraySerializer : ExclusiveSerializer
    {
        public const string ArrayTag = "array";
        public const string ItemTag = "item";
        public const string ElementTypeTag = "elementtype";

        public ArraySerializer(XmSerializerModel serializerModel) : base(serializerModel)
        {
            serializerModel.AddType(new TypeSerializingSettings(typeof(Array)));
        }

        public override bool CanDeserialize(string xmlName)
        {
            return xmlName.Equals(ArrayTag);
        }

        public override bool CanSerialize(object instance, Type type)
        {
            return type.IsArray;
        }

        public override object Deserialize(XElement xml)
        {
            var elements = xml.Elements(ItemTag).ToList();
            var elementTypeAlias = (string)xml.Attribute(ElementTypeTag);
            var elementType = this.SerializerModel.ParseTypeAlias(elementTypeAlias);
            var ret = Array.CreateInstance(elementType, elements.Count);
            this.SerializerModel.RegisterInstance(ret, xml);

            for (var i = 0; i < elements.Count; i++)
                ret.SetValue(this.SerializerModel.DeserializeSubObject(elements[i].Elements().FirstOrDefault()), i);

            return ret;
        }

        public override XElement Serialize(object instance, Type type)
        {
            var xml = new XElement(ArrayTag);
            this.SerializerModel.RegisterXml(xml, instance);
            var elementType = type.GetElementType();
            var elementTypeAlias = this.SerializerModel.GetTypeAlias(elementType);
            xml.SetAttributeValue(ElementTypeTag, elementTypeAlias);
            foreach (var element in (IEnumerable)instance)
                xml.Add(new XElement(ItemTag, this.SerializerModel.SerializeSubObject(element)));

            return xml;
        }
    }
}