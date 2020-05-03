using System;
using System.Xml.Linq;

namespace XmSerializer
{
    public interface IExclusiveSerializer
    {
        bool CanSerialize(object instance, Type type);
        bool CanDeserialize(string xmlName);
        object Deserialize(XElement xml);
        XElement Serialize(object instance, Type type);
    }
}