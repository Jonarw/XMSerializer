using System;
using System.Xml.Linq;

namespace XmSerializer.Serializers
{
    public abstract class ExclusiveSerializer : IExclusiveSerializer
    {
        protected ExclusiveSerializer(XmSerializerModel serializerModel)
        {
            serializerModel.ExclusiveSerializers.Add(this);
            this.SerializerModel = serializerModel;
        }

        public XmSerializerModel SerializerModel { get; }
        public abstract bool CanDeserialize(string xmlName);
        public abstract bool CanSerialize(object instance, Type type);

        /// <remarks>Call RegisterInstance on <see cref="SerializerModel"/> for reference tracking.</remarks>
        public abstract object Deserialize(XElement xml);

        /// <remarks>Call RegisterXml on <see cref="SerializerModel"/> for reference tracking.</remarks>
        public abstract XElement Serialize(object instance, Type type);
    }
}