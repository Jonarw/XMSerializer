using System;
using System.Numerics;
using System.Xml.Linq;

namespace XmSerializer.Serializers
{
    public class ComplexSerializer : ExclusiveSerializer
    {
        private const string ComplexTag = "complex";
        private const string RealTag = "re";
        private const string ImaginaryTag = "im";

        public ComplexSerializer(XmSerializerModel serializerModel) : base(serializerModel)
        {
        }

        public override bool CanDeserialize(string xmlName)
        {
            return xmlName == ComplexTag;
        }

        public override bool CanSerialize(object instance, Type type)
        {
            return type == typeof(Complex);
        }

        public override object Deserialize(XElement xml)
        {
            var real = (double)xml.Attribute(RealTag);
            var imaginary = (double)xml.Attribute(ImaginaryTag);
            return new Complex(real, imaginary);
        }

        public override XElement Serialize(object instance, Type type)
        {
            var element = new XElement(ComplexTag);
            var complex = (Complex)instance;
            element.SetAttributeValue(RealTag, complex.Real);
            element.SetAttributeValue(ImaginaryTag, complex.Imaginary);
            return element;
        }
    }
}
