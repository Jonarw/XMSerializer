using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using XmSerializer.Extensions;

namespace XmSerializer.Serializers
{
    public class ValueTupleSerializer : ExclusiveSerializer
    {
        private const string TupleTag = "valuetuple";
        private const string TypeTag = "type";

        private static readonly HashSet<Type> TupleTypes = new HashSet<Type>
        {
            typeof(ValueTuple<>),
            typeof(ValueTuple<,>),
            typeof(ValueTuple<,,>),
            typeof(ValueTuple<,,,>),
            typeof(ValueTuple<,,,,>),
            typeof(ValueTuple<,,,,,>),
            typeof(ValueTuple<,,,,,,>),
            typeof(ValueTuple<,,,,,,,>)
        };

        public ValueTupleSerializer(XmSerializerModel serializerModel) : base(serializerModel)
        {
            foreach (var tupleType in TupleTypes)
                serializerModel.AddType(new TypeSerializingSettings(tupleType));
        }

        public override bool CanDeserialize(string xmlName)
        {
            return string.Equals(xmlName, TupleTag);
        }

        public override bool CanSerialize(object instance, Type type)
        {
            return type.IsGenericType && TupleTypes.Contains(type.GetGenericTypeDefinition());
        }

        public override object Deserialize(XElement xml)
        {
            var items = xml.Elements().Select(e => this.SerializerModel.DeserializeSubObject(e)).ToArray();
            var type = this.SerializerModel.ParseTypeAlias((string)xml.Attribute(TypeTag));
            return Activator.CreateInstance(type, items);
        }

        public override XElement Serialize(object instance, Type type)
        {
            var ret = new XElement(TupleTag);
            ret.SetAttributeValue(TypeTag, this.SerializerModel.GetTypeAlias(type));

            var len = Math.Min(type.GenericTypeArguments.Length, 7);

            for (var i = 0; i < len; i++)
                ret.Add(this.SerializerModel.SerializeSubObject(instance.GetFieldValue<object>($"Item{i + 1}")));

            if (type.GenericTypeArguments.Length == 8)
                ret.Add(this.SerializerModel.SerializeSubObject(instance.GetFieldValue<object>("Rest")));

            return ret;
        }
    }
}