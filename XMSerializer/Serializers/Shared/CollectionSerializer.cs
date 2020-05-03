using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Xml.Linq;

namespace XmSerializer.Serializers
{
    public class CollectionSerializer : SharedSerializer
    {
        public const string ItemTag = "item";

        public CollectionSerializer(XmSerializerModel serializerModel) : base(serializerModel)
        {
            serializerModel.AddType(new TypeSerializingSettings(typeof(List<>)) {IsCollection = true, SkipConstructor = false});
            serializerModel.AddType(new TypeSerializingSettings(typeof(Collection<>)) {IsCollection = true, SkipConstructor = false});
            serializerModel.AddType(new TypeSerializingSettings(typeof(ArrayList)) {IsCollection = true, SkipConstructor = false});
            serializerModel.AddType(new TypeSerializingSettings(typeof(SortedSet<>)) {IsCollection = true, SkipConstructor = false});

            serializerModel.AddType(new TypeSerializingSettings(typeof(IList)));
            serializerModel.AddType(new TypeSerializingSettings(typeof(IList<>)));
            serializerModel.AddType(new TypeSerializingSettings(typeof(IEnumerable)));
            serializerModel.AddType(new TypeSerializingSettings(typeof(IEnumerable<>)));
            serializerModel.AddType(new TypeSerializingSettings(typeof(IReadOnlyList<>)));
            serializerModel.AddType(new TypeSerializingSettings(typeof(IReadOnlyCollection<>)));
            serializerModel.AddType(new TypeSerializingSettings(typeof(ICollection)));
            serializerModel.AddType(new TypeSerializingSettings(typeof(ICollection<>)));
            serializerModel.AddType(new TypeSerializingSettings(typeof(ISet<>)));

            var type = new TypeSerializingSettings(typeof(ReadOnlyCollection<>));
            type.AddMember(new MemberSerializingSettings("list", "InternalList", false));
            serializerModel.AddType(type);

            type = new TypeSerializingSettings(typeof(ReadOnlyObservableCollection<>));
            type.AddMember(new MemberSerializingSettings("list", "InternalList", false));
            serializerModel.AddType(type);
        }

        public override bool CanSerialize(TypeSerializingSettings typeSettings)
        {
            return typeSettings.IsCollection;
        }

        public override void Deserialize(object instance, XElement xml, Type type, TypeSerializingSettings typeSettings)
        {
            if (xml.Elements().Any(e => !e.HasElements))
                Console.WriteLine("WARNING [malformed XML]: Empty item entries found in collection.");

            var elements = xml.Elements(ItemTag)
                .Where(e => e.HasElements)
                .Select(e => this.SerializerModel.DeserializeSubObject(e.Elements().First()));

            if (instance is IList il)
            {
                foreach (var element in elements)
                    il.Add(element);

                return;
            }

            var addMethod = type.GetAllInstanceMethods().FirstOrDefault(m => m.Name == "Add" && m.GetParameters().Length == 1);
            if (addMethod == null)
            {
                Console.WriteLine($"WARNING: No suitable Add method found on {type}. Discarding collection content.");
                return;
            }

            foreach (var element in elements)
                addMethod.Invoke(instance, new[] {element});
        }

        public override void Serialize(object instance, XElement xml, Type type, TypeSerializingSettings typeSettingsr)
        {
            foreach (var element in (IEnumerable)instance)
                xml.Add(new XElement(ItemTag, this.SerializerModel.SerializeSubObject(element)));
        }
    }
}