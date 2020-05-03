using System;
using System.Xml.Linq;

namespace XmSerializer.Serializers
{
    public abstract class ClassSpecificSharedSerializer<T> : SharedSerializer
    {
        protected ClassSpecificSharedSerializer(XmSerializerModel serializerModel) : base(serializerModel)
        {
        }

        public sealed override bool CanSerialize(TypeSerializingSettings typeSettings)
        {
            return typeof(T).IsAssignableFrom(typeSettings.Type);
        }

        public sealed override void Deserialize(object instance, XElement xml, Type type, TypeSerializingSettings typeSettings)
        {
            this.Deserialize((T)instance, xml);
        }

        public sealed override void Serialize(object instance, XElement xml, Type type, TypeSerializingSettings typeSettings)
        {
            this.Serialize((T) instance, xml);
        }

        protected abstract void Deserialize(T instance, XElement xml);
        protected abstract void Serialize(T instance, XElement xml);
    }
}