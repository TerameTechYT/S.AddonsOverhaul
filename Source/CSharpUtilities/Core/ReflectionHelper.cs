using System;
using System.Reflection;

namespace CSharpUtilities.Core
{
    public class ReflectionHelper
    {
        public static FieldInfo GetStaticField(Type classType, string fieldName, Action<string> errorFunc = null)
        {
            var field = classType.GetField(fieldName,
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
            if (field == null)
                if (errorFunc != null)
                    errorFunc($"Field {fieldName} not found in type {classType.Name}");
                else
                    throw new Exception($"Field {fieldName} not found in type {classType.Name}");
            return field;
        }


        public static TCast ReadStaticField<TCast>(Type classType, string fieldName, object obj = null,
            Action<string> errorFunc = null)
        {
            return (TCast)GetStaticField(classType, fieldName, errorFunc).GetValue(obj);
        }

        public static void WriteStaticField(Type classType, string fieldName, object set, object obj = null,
            Action<string> errorFunc = null)
        {
            GetStaticField(classType, fieldName, errorFunc).SetValue(obj, set);
        }

        public static FieldInfo GetPrivateField(Type classType, string fieldName, Action<string> errorFunc = null)
        {
            var field = classType.GetField(fieldName,
                BindingFlags.NonPublic | BindingFlags.Instance);
            if (field == null)
                if (errorFunc != null)
                    errorFunc($"Field {fieldName} not found in type {classType.Name}");
                else
                    throw new Exception($"Field {fieldName} not found in type {classType.Name}");
            return field;
        }

        public static TCast ReadPrivateField<TCast>(Type classType, string fieldName, object obj = null,
            Action<string> errorFunc = null)
        {
            return (TCast)GetPrivateField(classType, fieldName, errorFunc).GetValue(obj);
        }

        public static void WritePrivateField(Type classType, string fieldName, object set, object obj = null,
            Action<string> errorFunc = null)
        {
            GetPrivateField(classType, fieldName, errorFunc).SetValue(obj, set);
        }

        public static MethodInfo GetStaticMethod(Type classType, string methodName, Action<string> errorFunc = null)
        {
            var method = classType.GetMethod(methodName,
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
            if (method == null)
                if (errorFunc != null)
                    errorFunc($"Method {methodName} not found in type {classType.Name}");
                else
                    throw new Exception($"Method {methodName} not found in type {classType.Name}");
            return method;
        }

        public static object InvokeStaticMethod(Type classType, string methodName, object obj = null,
            object[] param = null, Action<string> errorFunc = null)
        {
            return GetStaticMethod(classType, methodName, errorFunc).Invoke(obj, param);
        }

        public static MethodInfo GetPrivateMethod(Type classType, string methodName, Action<string> errorFunc = null)
        {
            var method = classType.GetMethod(methodName,
                BindingFlags.NonPublic | BindingFlags.Instance);
            if (method == null)
                if (errorFunc != null)
                    errorFunc($"Method {methodName} not found in type {classType.Name}");
                else
                    throw new Exception($"Method {methodName} not found in type {classType.Name}");
            return method;
        }

        public static object InvokePrivateMethod(Type classType, string methodName, object obj = null,
            object[] param = null, Action<string> errorFunc = null)
        {
            return GetPrivateMethod(classType, methodName, errorFunc).Invoke(obj, param);
        }

        public static MethodInfo GetMethod(Type classType, string methodName, Action<string> errorFunc = null)
        {
            var method = classType.GetMethod(methodName);
            if (method == null)
                if (errorFunc != null)
                    errorFunc($"Method {methodName} not found in type {classType.Name}");
                else
                    throw new Exception($"Method {methodName} not found in type {classType.Name}");
            return method;
        }

        public static object InvokeMethod(Type classType, string methodName, object obj = null,
            object[] param = null, Action<string> errorFunc = null)
        {
            return GetMethod(classType, methodName, errorFunc).Invoke(obj, param);
        }
    }
}