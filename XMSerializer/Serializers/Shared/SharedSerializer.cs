using System;
using System.Xml.Linq;

namespace XmSerializer.Serializers
{
    public abstract class SharedSerializer : ISharedSerializer
    {
        protected SharedSerializer(XmSerializerModel serializerModel)
        {
            this.SerializerModel = serializerModel;
            serializerModel.SharedSerializers.Add(this);
        }

        public XmSerializerModel SerializerModel { get; }

        public abstract bool CanSerialize(TypeSerializingSettings typeSettings);
        public abstract void Deserialize(object instance, XElement xml, Type type, TypeSerializingSettings typeSettings);
        public abstract void Serialize(object instance, XElement xml, Type type, TypeSerializingSettings typeSettings);
    }
}