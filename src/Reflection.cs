
using System;
using System.Reflection;

namespace RoboPhredDev.Shipbreaker.SixAxis
{
    public static class Reflection
    {
        public static void SetPrivateField(object instance, string fieldName, object value)
        {
            var field = instance.GetType().GetField(fieldName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (field == null)
            {
                throw new ArgumentException($"Type {instance.GetType().Name} does not have a field named {fieldName}");
            }
            field.SetValue(instance, value);
        }

        public static T GetPrivateField<T>(object instance, string fieldName)
        {
            var field = instance.GetType().GetField(fieldName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (field == null)
            {
                throw new ArgumentException($"Type {instance.GetType().Name} does not have a field named {fieldName}");
            }
            return (T)field.GetValue(instance);
        }

        public static T GetPrivateProperty<T>(object instance, string propertyName)
        {
            var prop = instance.GetType().GetProperty(propertyName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (prop == null)
            {
                throw new ArgumentException($"Type {instance.GetType().Name} does not have a property named {propertyName}");
            }
            return (T)prop.GetValue(instance);
        }

        public static void CallPrivateMethod(object instance, string methodName, params object[] parameters)
        {
            var method = instance.GetType().GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance);
            if (method == null)
            {
                throw new ArgumentException($"Type {instance.GetType().Name} does not have a method named {methodName}");
            }
            method.Invoke(instance, parameters);
        }
    }
}