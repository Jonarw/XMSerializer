using System;
using System.Xml.Linq;

namespace XmSerializer
{
    public interface ISharedSerializer
    {
        bool CanSerialize(TypeSerializingSettings typeSettings);
        void Serialize(object instance, XElement xml, Type type, TypeSerializingSettings typeSettings);
        void Deserialize(object instance, XElement xml, Type type, TypeSerializingSettings typeSettings);
    }
}