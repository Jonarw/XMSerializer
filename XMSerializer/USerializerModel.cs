using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Xml.Linq;
using XmSerializer.Serializers;
using XmSerializer.Extensions;

namespace XmSerializer
{
    public class XmSerializerModel
    {
        public const string ArraySuffix = "[]";
        public const char GenericDelimiterLeft = '{';
        public const char GenericDelimiterMiddle = ',';
        public const char GenericDelimiterRight = '}';
        public const string IdTag = "id";
        public const string ObjectTag = "object";
        public const string RefTag = "ref";
        public const string TypeTag = "type";
        private readonly object Lock = new object();

        private int noIdCounter;
        private int objectCounter;

        private IDictionary<object, int> objectDictionary;
        private IDictionary<int, object> objectDictionary2;

        public XmSerializerModel()
        {
            this.AddType(new TypeSerializingSettings(nameof(Object), typeof(object)));
            this.AddAssembly(typeof(SerializableTypeAttribute).Assembly);
            new NullSerializer(this);
            new NullableSerializer(this);
            new PrimitivesSerializer(this);
            new EnumSerializer(this);
            new ValueTupleSerializer(this);
            new TupleSerializer(this);
            new ArraySerializer(this);
            new MemberSerializer(this);
            new CollectionSerializer(this);
            new DictionarySerializer(this);
            new ComplexSerializer(this);
        }

        public IList<IExclusiveSerializer> ExclusiveSerializers { get; } = new List<IExclusiveSerializer>();

        public bool IgnoreMissingReferences { get; set; }
        public IReadOnlyDictionary<string, TypeSerializingSettings> RegisteredLegacyNames => this.LegacyNames;

        public IReadOnlyDictionary<string, TypeSerializingSettings> RegisteredNames => this.TypeDictionary2;

        public IReadOnlyDictionary<Type, TypeSerializingSettings> RegisteredTypes => this.TypeDictionary;

        public IList<ISharedSerializer> SharedSerializers { get; } = new List<ISharedSerializer>();

        public IReadOnlyList<TypeSerializingSettings> Types
        {
            set
            {
                foreach (var type in value)
                    this.AddType(type);
            }
        }

        private Dictionary<string, TypeSerializingSettings> LegacyNames { get; } = new Dictionary<string, TypeSerializingSettings>();

        private Dictionary<Type, TypeSerializingSettings> TypeDictionary { get; } = new Dictionary<Type, TypeSerializingSettings>();

        private Dictionary<string, TypeSerializingSettings> TypeDictionary2 { get; } = new Dictionary<string, TypeSerializingSettings>();

        public void AddAssembly(Assembly assembly)
        {
            foreach (var type in assembly.GetTypes().Where(t => Attribute.IsDefined(t, typeof(SerializableTypeAttribute), false) && !this.TypeDictionary.ContainsKey(t)))
                this.AddType(type);
        }

        public void AddType(TypeSerializingSettings typeSettings)
        {
            if (this.TypeDictionary.ContainsKey(typeSettings.Type))
                throw new Exception($"The type {typeSettings.Type} has already been added to the TypeDictionary.");

            this.TypeDictionary.Add(typeSettings.Type, typeSettings);
            this.TypeDictionary2.Add(typeSettings.Alias, typeSettings);
            foreach (var legacyName in typeSettings.LegacyNames)
                this.LegacyNames.Add(legacyName, typeSettings);
        }

        public void AddType(Type type)
        {
            var typeAtt = type.GetCustomAttribute<SerializableTypeAttribute>();
            if (typeAtt == null)
                throw new Exception("Type must have the SerializedTypeAttribute.");

            var typeName = typeAtt.Alias ?? type.Name;
            if (this.TypeDictionary2.ContainsKey(typeName))
                throw new Exception($"Duplicate type name {typeName}.");

            var settings = new TypeSerializingSettings(typeName, type)
            {
                IsCollection = typeAtt.IsCollection,
                SkipConstructor = typeAtt.SkipConstructor,
                IncludeAncestors = typeAtt.IncludeAncestors,
                IsDictionary = typeAtt.IsDictionary
            };

            if (typeAtt.LegacyName != null)
                settings.LegacyNames.Add(typeAtt.LegacyName);

            var fields = type.GetFields(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public).ToList();
            var properties = type.GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public).ToList();

            var implicitFields = Enumerable.Empty<FieldInfo>();
            if (typeAtt.ImplicitMemberFilter.HasFlag(MemberFilter.NonPublicFields))
                implicitFields = implicitFields.Concat(fields.Where(f => !f.IsPublic));

            if (typeAtt.ImplicitMemberFilter.HasFlag(MemberFilter.PublicFields))
                implicitFields = implicitFields.Concat(fields.Where(f => f.IsPublic));

            foreach (var field in implicitFields.Where(f => !Attribute.IsDefined(f, typeof(NotSerializedAttribute)) && !Attribute.IsDefined(f, typeof(SerializedAttribute))))
                settings.AddMember(new MemberSerializingSettings(field.Name, field.Name, false));

            var implicitProperties = Enumerable.Empty<PropertyInfo>();
            if (typeAtt.ImplicitMemberFilter.HasFlag(MemberFilter.NonPublicProperties))
                implicitProperties = implicitProperties.Concat(properties.Where(p => p.GetGetMethod() == null));

            if (typeAtt.ImplicitMemberFilter.HasFlag(MemberFilter.PublicProperties))
                implicitProperties = implicitProperties.Concat(properties.Where(p => p.GetGetMethod() != null));

            foreach (var property in implicitProperties.Where(p => !Attribute.IsDefined(p, typeof(NotSerializedAttribute)) && !Attribute.IsDefined(p, typeof(SerializedAttribute))))
                settings.AddMember(new MemberSerializingSettings(property.Name, property.Name, true));

            if (typeAtt.ImplicitMemberFilter.HasFlag(MemberFilter.Explicit))
            {
                var explicitFields = fields.Where(f => Attribute.IsDefined(f, typeof(SerializedAttribute)));

                foreach (var field in explicitFields)
                {
                    var fieldAtt = field.GetCustomAttribute<SerializedAttribute>();
                    settings.AddMember(CreateMemberSettings(field.Name, false, fieldAtt));
                }

                var explicitProperties = properties.Where(f => Attribute.IsDefined(f, typeof(SerializedAttribute)));

                foreach (var property in explicitProperties)
                {
                    var propertyAtt = property.GetCustomAttribute<SerializedAttribute>();
                    settings.AddMember(CreateMemberSettings(property.Name, true, propertyAtt));
                }
            }

            var onDiscoveryMethods = type.GetStaticMethods()
                .Where(m => Attribute.IsDefined(m, typeof(OnTypeDiscoveryAttribute), false));

            foreach (var method in onDiscoveryMethods)
            {
                var parameters = method.GetParameters();
                if (parameters.Length == 0)
                {
                    method.Invoke(null, null);
                }
                else if (parameters.Length == 1 && parameters[0].ParameterType == typeof(TypeSerializingSettings))
                {
                    method.Invoke(null, new object[] {settings});
                }
                else
                {
                    throw new Exception($"Method {method.Name} on type {type.Name} has an invalid signature and cannot be used as an OnTypeDiscovery method.");
                } 
            }

            this.AddType(settings);
        }

        public T DeepCopy<T>(T instance)
        {
            return this.Deserialize<T>(this.Serialize(instance));
        }

        public T Deserialize<T>(XElement xml, IDictionary<int, object> objectDictionary)
        {
            lock (this.Lock)
            {
                this.objectDictionary2 = objectDictionary;

                var ret = (T)this.DeserializeSubObject(xml);

                IEnumerable<(object obj, MethodInfo method, int priority)> GetOnDeserializedMethods()
                {
                    foreach (var obj in this.objectDictionary2.Values)
                    {
                        var methods = obj.GetType().GetAllInstanceMethods();
                        foreach (var method in methods)
                        {
                            if (Attribute.IsDefined(method, typeof(System.Runtime.Serialization.OnDeserializedAttribute), false))
                                yield return (obj, method, 0);
                            else if (Attribute.GetCustomAttribute(method, typeof(OnDeserializedAttribute), false) is OnDeserializedAttribute att)
                                yield return (obj, method, att.Priority);
                        }
                    }
                }

                foreach (var (obj, method, _) in GetOnDeserializedMethods().OrderBy(m => m.priority))
                    InvokeWithNullParameters(method, obj);

                this.objectDictionary2 = null;
                return ret;
            }
        }

        public T Deserialize<T>(XElement xml)
        {
            return this.Deserialize<T>(xml, new Dictionary<int, object>());
        }

        public object DeserializeSubObject(XElement xml)
        {
            var name = xml.Name.LocalName;

            if (string.Equals(name, RefTag))
            {
                if (this.objectDictionary2.TryGetValue((int)xml, out var reference))
                    return reference;

                if (this.IgnoreMissingReferences)
                    return null;

                throw new Exception($"Referenced object with id {(int)xml} not found!");
            }

            var ex = this.ExclusiveSerializers.FirstOrDefault(s => s.CanDeserialize(name));
            if (ex != null)
                return ex.Deserialize(xml);

            var ret = this.CreateObject(xml, out var type, out var typeSettings);

            foreach (var serializer in this.SharedSerializers.Where(s => s.CanSerialize(typeSettings)))
                serializer.Deserialize(ret, xml, type, typeSettings);

            this.InitializeMembers(ret);
            return ret;
        }

        public string GetTypeAlias(Type type)
        {
            if (!type.IsGenericType)
            {
                if (type.IsArray)
                {
                    if (type.GetArrayRank() > 1)
                        throw new Exception("Multi-dimensional arrays are not supported.");

                    return this.GetTypeAlias(type.GetElementType()) + ArraySuffix;
                }

                if (!this.TypeDictionary.TryGetValue(type, out var typeSettings))
                    throw new Exception($"Type {type} was not found in the type dictionary.");

                return typeSettings.Alias;
            }

            var genericType = type.GetGenericTypeDefinition();
            if (!this.TypeDictionary.TryGetValue(genericType, out var genTypeSettings))
                throw new Exception($"Type {genericType} was not found in the type dictionary.");

            return $"{genTypeSettings.Alias}" +
                   $"{GenericDelimiterLeft}" +
                   $"{string.Join(GenericDelimiterMiddle.ToString(), type.GenericTypeArguments.Select(this.GetTypeAlias))}" +
                   $"{GenericDelimiterRight}";
        }

        public TypeSerializingSettings GetTypeSettings(string typeString)
        {
            TypeSerializingSettings TryGetTypeSettings(string str)
            {
                if (this.TypeDictionary2.TryGetValue(str, out var ret))
                    return ret;

                if (this.LegacyNames.TryGetValue(str, out ret))
                    return ret;

                return null;
            }

            var ret = TryGetTypeSettings(typeString);
            if (ret != null)
                return ret;

            var split = typeString.Split('`');
            if (split.Length >= 2 && int.TryParse(split.Last(), out var number))
            {
                var length = split.Last().Length;
                var strWithoutNumber = typeString.Substring(0, typeString.Length - length - 1);
                for (var i = number - 1; i >= 1; i--)
                {
                    var str = $"{strWithoutNumber}`{i}";
                    ret = TryGetTypeSettings(str);
                    if (ret != null)
                        return ret;
                }

                ret = TryGetTypeSettings(strWithoutNumber);
                if (ret != null)
                    return ret;
            }

            throw new Exception($"Type alias {typeString} was not found in the type dictionary.");
        }

        public Type ParseTypeAlias(string typeString)
        {
            if (!typeString.Contains(GenericDelimiterLeft))
            {
                if (typeString.EndsWith(ArraySuffix))
                {
                    var elementTypeAlias = typeString.Substring(0, typeString.Length - ArraySuffix.Length);
                    return this.ParseTypeAlias(elementTypeAlias).MakeArrayType();
                }

                var typeSettings = this.GetTypeSettings(typeString);
                return typeSettings.Type;
            }

            var genericTypeStringLength = typeString.IndexOf(GenericDelimiterLeft);
            var typeArgumentString = typeString.Substring(genericTypeStringLength + 1, typeString.Length - genericTypeStringLength - 2);
            var typeArgumentStrings = new List<string>();

            var subLevel = 0;
            var start = 0;
            int i;
            for (i = 0; i < typeArgumentString.Length; i++)
            {
                if (typeArgumentString[i] == GenericDelimiterLeft)
                {
                    subLevel++;
                }
                else if (typeArgumentString[i] == GenericDelimiterRight)
                {
                    subLevel--;
                }
                else if (typeArgumentString[i] == GenericDelimiterMiddle && subLevel == 0)
                {
                    typeArgumentStrings.Add(typeArgumentString.Substring(start, i - start));
                    start = i + 1;
                }
            }

            typeArgumentStrings.Add(typeArgumentString.Substring(start, i - start));

            var argumentTypes = typeArgumentStrings.Select(this.ParseTypeAlias).ToArray();
            var genericTypeString = typeString.Substring(0, genericTypeStringLength);
            var genericType = this.GetTypeSettings(genericTypeString).Type;
            var numberOfRequiredParameters = genericType.GetGenericArguments().Length;
            if (numberOfRequiredParameters == argumentTypes.Length)
                return genericType.MakeGenericType(argumentTypes);

            if (numberOfRequiredParameters == 0)
                return genericType;

            if (numberOfRequiredParameters < argumentTypes.Length)
                return genericType.MakeGenericType(argumentTypes.Take(numberOfRequiredParameters).ToArray());

            throw new Exception($"Can't construct type {genericType}: Needs {numberOfRequiredParameters} type parameters, only {argumentTypes.Length} available.");
        }

        public void RegisterInstance(object instance, XElement xml)
        {
            var idAttribute = xml.Attribute(IdTag);
            if (idAttribute != null)
            {
                var id = (int)idAttribute;
                if (!this.objectDictionary2.ContainsKey(id))
                    this.objectDictionary2.Add(id, instance);
                else
                    this.objectDictionary2.Add(int.MaxValue - this.noIdCounter++, instance);
            }

            foreach (var method in instance.GetType().GetAllInstanceMethods()
                .Where(m => Attribute.IsDefined(m, typeof(OnDeserializingAttribute), false) || Attribute.IsDefined(m, typeof(System.Runtime.Serialization.OnDeserializingAttribute), false)))
            {
                InvokeWithNullParameters(method, instance);
            }
        }

        public void RegisterXml(XElement xml, object instance)
        {
            if (instance.GetType().IsClass)
            {
                var id = this.objectCounter++;
                xml.SetAttributeValue(IdTag, id);
                this.objectDictionary.Add(instance, id);
            }

            foreach (var method in instance.GetType().GetAllInstanceMethods()
                .Where(m => Attribute.IsDefined(m, typeof(OnSerializingAttribute), false) || Attribute.IsDefined(m, typeof(System.Runtime.Serialization.OnSerializingAttribute), false)))
            {
                InvokeWithNullParameters(method, instance);
            }
        }

        public XElement Serialize(object instance, IDictionary<object, int> objectDictionary)
        {
            lock (this.Lock)
            {
                this.objectDictionary = objectDictionary;
                this.objectCounter = this.objectDictionary.Count + 1;

                var ret = this.SerializeSubObject(instance);
                foreach (var o in this.objectDictionary.Keys)
                {
                    foreach (var method in o.GetType().GetAllInstanceMethods()
                        .Where(m => Attribute.IsDefined(m, typeof(OnSerializedAttribute), false) || Attribute.IsDefined(m, typeof(System.Runtime.Serialization.OnSerializedAttribute), false)))
                    {
                        InvokeWithNullParameters(method, o);
                    }
                }

                this.objectDictionary = null;
                return ret;
            }
        }

        public XElement Serialize(object instance)
        {
            return this.Serialize(instance, new Dictionary<object, int>());
        }

        public XElement SerializeSubObject(object instance)
        {
            var type = instance?.GetType();

            if (type?.IsClass == true && this.objectDictionary.ContainsKey(instance))
                return new XElement(RefTag, this.objectDictionary[instance]);

            var ex = this.ExclusiveSerializers.FirstOrDefault(s => s.CanSerialize(instance, type));
            if (ex != null)
                return ex.Serialize(instance, type);

            var ret = this.CreateObjectContainer(instance, out type, out var typeSettings);

            foreach (var serializer in this.SharedSerializers.Where(s => s.CanSerialize(typeSettings)))
                serializer.Serialize(instance, ret, type, typeSettings);

            return ret;
        }

        internal TypeSerializingSettings GetTypeSettings(Type type)
        {
            var ret = this.TryGetTypeSettings(type);
            if (ret == null)
                throw new Exception($"The type {type} was not found in the type dictionary.");

            return ret;
        }

        internal TypeSerializingSettings TryGetTypeSettings(Type type)
        {
            var lookupType = type.IsGenericType ? type.GetGenericTypeDefinition() : type;
            this.TypeDictionary.TryGetValue(lookupType, out var ret);
            return ret;
        }

        private static MemberSerializingSettings CreateMemberSettings(string name, bool isProperty, SerializedAttribute att)
        {
            var ret = new MemberSerializingSettings(name, att.Alias ?? name, isProperty)
            {
                SkipInstantiating = att.SkipInstantiating,
                DefaultValue = att.DefaultValue,
                Obfuscate = att.Obfuscate,
                Serialize = att.Serialize,
                Deserialize = att.Deserialize
            };

            if (att.LegacyName != null)
                ret.LegacyNames.Add(att.LegacyName);

            return ret;
        }

        private static void InvokeWithNullParameters(MethodInfo method, object instance)
        {
            var noParamters = method.GetParameters().Length;
            method.Invoke(instance, new object[noParamters]);
        }

        private object CreateObject(XElement xml, out Type type, out TypeSerializingSettings typeSettings)
        {
            var typeAlias = (string)xml.Attribute(TypeTag);
            type = this.ParseTypeAlias(typeAlias);
            typeSettings = this.GetTypeSettings(type);
            var ret = typeSettings.SkipConstructor ? FormatterServices.GetUninitializedObject(type) : Activator.CreateInstance(type);
            this.RegisterInstance(ret, xml);
            return ret;
        }

        private XElement CreateObjectContainer(object instance, out Type type, out TypeSerializingSettings typeSettings)
        {
            type = instance.GetType();
            typeSettings = this.GetTypeSettings(type);
            var typeAlias = this.GetTypeAlias(type);
            var ret = new XElement(ObjectTag);
            this.RegisterXml(ret, instance);

            ret.SetAttributeValue(TypeTag, typeAlias);
            return ret;
        }

        private void InitializeMembers(object obj)
        {
            var type = obj.GetType();
            var attributeType = typeof(InitializeAfterDeserializationAttribute);
            var fields = type.GetAllInstanceFields()
                .Select(f => (attribute: (InitializeAfterDeserializationAttribute)Attribute.GetCustomAttribute(f, attributeType), field: f))
                .Where(tuple => tuple.attribute != null);

            foreach (var (attribute, field) in fields)
            {
                var initType = attribute.InitializationType ?? field.FieldType;
                if (initType.IsAbstract)
                    throw new Exception($"Cannot initialize abstract type {initType}.");

                if (!field.FieldType.IsAssignableFrom(initType))
                    throw new Exception($"Cannot assign {initType} to {field.FieldType}.");

                object initValue;
                try
                {
                    initValue = Activator.CreateInstance(initType, true);
                }
                catch (MissingMethodException e)
                {
                    throw new Exception($"No parameterless constructor found on {initType}", e);
                }

                field.SetValue(obj, initValue);
            }
        }
    }
}