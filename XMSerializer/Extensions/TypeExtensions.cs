using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace XmSerializer.Extensions
{
    public static class TypeExtensions
    {
        private const BindingFlags instanceFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly;

        public static IEnumerable<FieldInfo> GetAllInstanceFields(this Type type)
        {
            foreach (var field in type.GetFields(instanceFlags))
            {
                yield return field;
            }

            foreach (var ancestorType in type.GetAncestors())
            {
                foreach (var field in ancestorType.GetFields(instanceFlags))
                {
                    yield return field;
                }
            }
        }

        public static IEnumerable<MethodInfo> GetAllInstanceMethods(this Type type)
        {
            foreach (var method in type.GetMethods(instanceFlags))
            {
                yield return method;
            }

            foreach (var ancestorType in type.GetAncestors())
            {
                foreach (var method in ancestorType.GetMethods(instanceFlags))
                {
                    yield return method;
                }
            }
        }

        public static IEnumerable<PropertyInfo> GetAllInstanceProperties(this Type type)
        {
            foreach (var property in type.GetProperties(instanceFlags))
            {
                yield return property;
            }

            foreach (var ancestorType in type.GetAncestors())
            {
                foreach (var property in ancestorType.GetProperties(instanceFlags))
                {
                    yield return property;
                }
            }
        }

        public static IEnumerable<Type> GetAncestors(this Type type)
        {
            if (type.GetTypeInfo().IsInterface)
                throw new InvalidOperationException($"{nameof(type)} can't be an interface type.");

            while (type != typeof(object))
            {
                type = type.GetTypeInfo().BaseType;
                yield return type;
            }
        }

        public static FieldInfo GetInstanceField(this Type type, string name)
        {
            return type.GetAllInstanceFields().FirstOrDefault(field => field.Name == name);
        }

        public static MethodInfo GetInstanceMethod(this Type type, string name)
        {
            return type.GetAllInstanceMethods().FirstOrDefault(method => method.Name == name);
        }

        public static PropertyInfo GetInstanceProperty(this Type type, string name)
        {
            return type.GetAllInstanceProperties().FirstOrDefault(property => property.Name == name);
        }
    }
}