using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace XmSerializer.Serializers
{
    public class DictionarySerializer : SharedSerializer
    {
        private const string ItemTag = "entry";

        public DictionarySerializer(XmSerializerModel serializerModel) : base(serializerModel)
        {
            serializerModel.AddType(new TypeSerializingSettings(typeof(Dictionary<,>)) {IsDictionary = true, SkipConstructor = false});
            serializerModel.AddType(new TypeSerializingSettings(typeof(SortedList<,>)) {IsDictionary = true, SkipConstructor = false});
            serializerModel.AddType(new TypeSerializingSettings(typeof(SortedDictionary<,>)) {IsDictionary = true, SkipConstructor = false});
            serializerModel.AddType(new TypeSerializingSettings(typeof(ConcurrentDictionary<,>)) {IsDictionary = true, SkipConstructor = false});

            serializerModel.AddType(new TypeSerializingSettings(typeof(IDictionary)));
            serializerModel.AddType(new TypeSerializingSettings(typeof(IDictionary<,>)));
            serializerModel.AddType(new TypeSerializingSettings(typeof(IReadOnlyDictionary<,>)));
        }

        public override bool CanSerialize(TypeSerializingSettings typeSettings)
        {
            return typeSettings.IsDictionary;
        }

        public override void Deserialize(object instance, XElement xml, Type type, TypeSerializingSettings typeSettings)
        {
            if (!(instance is IDictionary dict))
            {
                Console.WriteLine($"WARNING [incompatibility]: Target type {type} does not implement IDictionary. Discarding dictionary content.");
                return;
            }

            foreach (var element in xml.Elements(ItemTag))
            {
                var key = this.SerializerModel.DeserializeSubObject(element.Elements().First());
                var value = this.SerializerModel.DeserializeSubObject(element.Elements().Skip(1).First());
                dict.Add(key, value);
            }
        }

        public override void Serialize(object instance, XElement xml, Type type, TypeSerializingSettings typeSettings)
        {
            var dict = instance as IDictionary ?? throw new Exception("Instance must implement IDictionary.");
            foreach (DictionaryEntry element in dict)
            {
                var item = new XElement(ItemTag);
                item.Add(this.SerializerModel.SerializeSubObject(element.Key));
                item.Add(this.SerializerModel.SerializeSubObject(element.Value));
                xml.Add(item);
            }
        }
    }
}