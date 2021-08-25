using System;

namespace XmSerializer.Extensions
{
    public static class ObjectExtensions
    {
        public static T GetFieldValue<T>(this object obj, string fieldName)
        {
            if (obj is null)
                throw new ArgumentNullException(nameof(obj));
            if (string.IsNullOrEmpty(fieldName))
                throw new ArgumentException($"{nameof(fieldName)} cannot be null or empty.", nameof(fieldName));

            var type = obj.GetType();
            var fieldInfo = type.GetInstanceField(fieldName);
            if (fieldInfo == null)
                throw new ArgumentException($"Field {fieldName} not found on type {type.Name}.");

            var value = fieldInfo.GetValue(obj);
            if (value is T valueT)
                return valueT;

            if (value is null)
                return default;

            throw new Exception($"Value of field {fieldName} on type {type.Name} is of type {value.GetType().Name} and cannot be cast to {typeof(T).Name}.");
        }

        public static T GetPropertyValue<T>(this object obj, string propertyName)
        {
            if (obj is null)
                throw new ArgumentNullException(nameof(obj));
            if (string.IsNullOrEmpty(propertyName))
                throw new ArgumentException($"{nameof(propertyName)} cannot be null or empty.", nameof(propertyName));

            var type = obj.GetType();
            var propertyInfo = type.GetInstanceProperty(propertyName);
            if (propertyInfo == null)
                throw new ArgumentException($"Property {propertyName} not found on type {type.Name}.");

            if (!propertyInfo.CanRead)
                throw new Exception($"Property {propertyName} on type {type.Name} does not have a Get method.");

            var value = propertyInfo.GetValue(obj);
            if (value is T valueT)
                return valueT;

            if (value is null)
                return default;

            throw new Exception($"Value of property {propertyName} on type {type.Name} is of type {value.GetType().Name} and cannot be cast to {typeof(T).Name}.");
        }

        public static bool TryGetFieldValue<T>(this object o, string fieldName, out T value)
        {
            var fieldInfo = o?.GetType().GetInstanceField(fieldName);
            if (fieldInfo == null)
            {
                value = default;
                return false;
            }

            var fieldValue = fieldInfo.GetValue(o);
            if (fieldValue == null || typeof(T).IsAssignableFrom(fieldValue.GetType()))
            {
                value = (T)fieldValue;
                return true;
            }

            value = default;
            return false;
        }

        public static bool TryGetMemberValue<T>(this object o, string memberName, bool isProperty, out T value)
        {
            return isProperty ? o.TryGetPropertyValue(memberName, out value) : o.TryGetFieldValue(memberName, out value);
        }

        public static bool TryGetPropertyValue<T>(this object o, string propertyName, out T value)
        {
            var propertyInfo = o?.GetType().GetInstanceProperty(propertyName);
            if (propertyInfo == null || !propertyInfo.CanRead)
            {
                value = default;
                return false;
            }

            var propertyValue = propertyInfo.GetValue(o);
            if (propertyValue == null || typeof(T).IsAssignableFrom(propertyValue.GetType()))
            {
                value = (T)propertyValue;
                return true;
            }

            value = default;
            return false;
        }

        public static bool TrySetFieldValue(this object o, string fieldName, object value)
        {
            var fieldInfo = o?.GetType().GetInstanceField(fieldName);
            if (fieldInfo == null || (value != null && !fieldInfo.FieldType.IsAssignableFrom(value.GetType())))
                return false;

            fieldInfo.SetValue(o, value);
            return true;
        }

        public static bool TrySetMemberValue(this object o, string memberName, object value, bool isProperty)
        {
            return isProperty ? o.TrySetPropertyValue(memberName, value) : o.TrySetFieldValue(memberName, value);
        }

        public static bool TrySetPropertyValue(this object o, string propertyName, object value)
        {
            var prop = o?.GetType().GetInstanceProperty(propertyName);
            if (prop == null || !prop.CanWrite || (value != null && !prop.PropertyType.IsAssignableFrom(value.GetType())))
                return false;

            prop.SetValue(o, value);
            return true;
        }
    }
}