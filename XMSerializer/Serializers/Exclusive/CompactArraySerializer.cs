using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml.Linq;

namespace XmSerializer.Serializers
{
    public class CompactArraySerializer : ExclusiveSerializer
    {
        public const string ArrayTag = "compactarray";
        public const string ElementTypeTag = "elementtype";
        public const string ContentTag = "content";

        public static readonly Dictionary<Type, int> PrimitiveSizes = new Dictionary<Type, int>
        {
            {typeof(double), sizeof(double)},
            {typeof(int), sizeof(int)},
            {typeof(byte), sizeof(byte)},
            {typeof(sbyte), sizeof(sbyte)},
            {typeof(uint), sizeof(uint)},
            {typeof(long), sizeof(long)},
            {typeof(ulong), sizeof(ulong)},
            {typeof(bool), sizeof(bool)},
            {typeof(float), sizeof(float)},
            {typeof(short), sizeof(short)},
            {typeof(ushort), sizeof(ushort)},
        };

        public CompactArraySerializer(XmSerializerModel serializerModel) : base(serializerModel)
        {
            for (var i = 0; i < serializerModel.ExclusiveSerializers.Count; i++)
            {
                // make sure we have higher priority than a regular ArraySerializer
                if (serializerModel.ExclusiveSerializers[i] is ArraySerializer)
                {
                    serializerModel.ExclusiveSerializers.Remove(this);
                    serializerModel.ExclusiveSerializers.Insert(i, this);
                    break;
                }
            }
        }

        public override bool CanDeserialize(string xmlName)
        {
            return xmlName.Equals(ArrayTag);
        }

        public override bool CanSerialize(object instance, Type type)
        {
            if (!type.IsArray)
                return false;

            var elementtype = type.GetElementType();
            return elementtype != null && PrimitiveSizes.ContainsKey(elementtype);
        }

        public override object Deserialize(XElement xml)
        {
            var base64String = (string)xml.Attribute(ContentTag);
            var bytes = Convert.FromBase64String(base64String);

            var elementTypeAlias = (string)xml.Attribute(ElementTypeTag);
            var elementType = this.SerializerModel.ParseTypeAlias(elementTypeAlias);

            var size = bytes.Length / PrimitiveSizes[elementType];

            var ret = Array.CreateInstance(elementType, size);
            this.SerializerModel.RegisterInstance(ret, xml);

            Buffer.BlockCopy(bytes, 0, ret, 0, bytes.Length);

            return ret;
        }

        public override XElement Serialize(object instance, Type type)
        {
            var xml = new XElement(ArrayTag);
            this.SerializerModel.RegisterXml(xml, instance);

            var elementType = type.GetElementType();
            Debug.Assert(elementType != null, nameof(elementType) + " != null");
            var elementTypeAlias = this.SerializerModel.GetTypeAlias(elementType);
            xml.SetAttributeValue(ElementTypeTag, elementTypeAlias);

            var array = (Array)instance;
            var size = array.Length * PrimitiveSizes[elementType];
            var bytes = new byte[size];
            Buffer.BlockCopy(array, 0, bytes, 0, size);
            var base64String = Convert.ToBase64String(bytes);
            xml.SetAttributeValue(ContentTag, base64String);

            return xml;
        }
    }
}