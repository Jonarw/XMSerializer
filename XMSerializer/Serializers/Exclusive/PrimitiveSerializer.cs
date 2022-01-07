using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace XmSerializer.Serializers
{
    public class PrimitivesSerializer : ExclusiveSerializer
    {
        public const string DoubleTag = "double";
        public const string IntTag = "int";
        public const string BoolTag = "bool";
        public const string UintTag = "uint";
        public const string LongTag = "long";
        public const string UlongTag = "ulong";
        public const string StringTag = "string";
        public const string ShortTag = "short";
        public const string ByteTag = "byte";
        public const string DateTimeTag = "DateTime";
        public const string TimeSpanTag = "TimeSpan";
        public const string DateTimeOffsetTag = "DateTimeOffset";
        public const string DecimalTag = "decimal";
        public const string FloatTag = "float";
        public const string ValueTag = "value";
        public const string CharTag = "char";
        public static readonly Dictionary<Type, string> Primitives2;

        public static readonly Dictionary<string, Type> Primitives = new Dictionary<string, Type>
        {
            {DoubleTag, typeof(double)},
            {IntTag, typeof(int)},
            {ByteTag, typeof(byte)},
            {StringTag, typeof(string)},
            {UintTag, typeof(uint)},
            {LongTag, typeof(long)},
            {UlongTag, typeof(ulong)},
            {BoolTag, typeof(bool)},
            {FloatTag, typeof(float)},
            {DecimalTag, typeof(decimal)},
            {DateTimeTag, typeof(DateTime)},
            {DateTimeOffsetTag, typeof(DateTimeOffset)},
            {TimeSpanTag, typeof(TimeSpan)},
            {ShortTag, typeof(short)},
            {CharTag, typeof(char)},
        };

        static PrimitivesSerializer()
        {
            Primitives2 = new Dictionary<Type, string>();
            foreach (var primitive in Primitives)
                Primitives2.Add(primitive.Value, primitive.Key);
        }

        public PrimitivesSerializer(XmSerializerModel serializerModel) : base(serializerModel)
        {
            foreach (var primitive in Primitives)
                serializerModel.AddType(new TypeSerializingSettings(primitive.Key, primitive.Value));
        }

        public override bool CanDeserialize(string xmlName)
        {
            return Primitives.ContainsKey(xmlName);
        }

        public override bool CanSerialize(object instance, Type type)
        {
            return Primitives2.ContainsKey(type);
        }

        public override object Deserialize(XElement xml)
        {
            var name = xml.Name.LocalName;
            return name switch
            {
                StringTag => (string)xml.Attribute(ValueTag),
                IntTag => (int)xml.Attribute(ValueTag),
                ByteTag => (byte)(int)xml.Attribute(ValueTag),
                DoubleTag => (double)xml.Attribute(ValueTag),
                BoolTag => (bool)xml.Attribute(ValueTag),
                LongTag => (long)xml.Attribute(ValueTag),
                DecimalTag => (decimal)xml.Attribute(ValueTag),
                TimeSpanTag => (TimeSpan)xml.Attribute(ValueTag),
                DateTimeTag => (DateTime)xml.Attribute(ValueTag),
                UintTag => (uint)xml.Attribute(ValueTag),
                UlongTag => (ulong)xml.Attribute(ValueTag),
                FloatTag => (float)xml.Attribute(ValueTag),
                DateTimeOffsetTag => (DateTimeOffset)xml.Attribute(ValueTag),
                ShortTag => (short)xml.Attribute(ValueTag),
                CharTag => (char)(int)xml.Attribute(ValueTag),
                _ => throw new Exception($"Primitive {name} not found."),
            };
        }

        public override XElement Serialize(object instance, Type type)
        {
            var ret = new XElement(Primitives2[type]);
            if (instance is char ch)
            {
                ret.SetAttributeValue(ValueTag, (int)ch);
            }
            else
            {
                ret.SetAttributeValue(ValueTag, instance);
            }

            return ret;
        }
    }
}