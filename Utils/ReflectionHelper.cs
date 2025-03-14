using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace UCParser.Utils
{
    public static class ReflectionHelper
    {
        private static readonly Dictionary<Type, string> PrimitiveTypeMap = new Dictionary<Type, string>
        {
            { typeof(void), "void" },
            { typeof(bool), "bool" },
            { typeof(byte), "byte" },
            { typeof(sbyte), "sbyte" },
            { typeof(char), "char" },
            { typeof(decimal), "decimal" },
            { typeof(double), "double" },
            { typeof(float), "float" },
            { typeof(int), "int" },
            { typeof(uint), "uint" },
            { typeof(long), "long" },
            { typeof(ulong), "ulong" },
            { typeof(short), "short" },
            { typeof(ushort), "ushort" },
            { typeof(string), "string" },
            { typeof(object), "object" }
        };

        public static string GetTypeName(Type type)
        {
            if (type == null)
                return "void";

            if (PrimitiveTypeMap.TryGetValue(type, out string typeName))
                return typeName;

            if (type.IsArray)
                return GetTypeName(type.GetElementType()) + "[]";

            if (type.IsByRef)
                return GetTypeName(type.GetElementType());

            if (type.IsGenericType)
            {
                Type[] genericArgs = type.GetGenericArguments();
                string genericArgsStr = string.Join(", ", genericArgs.Select(GetTypeName));

                string baseTypeName = type.Name;
                int idx = baseTypeName.IndexOf('`');
                if (idx > 0)
                    baseTypeName = baseTypeName.Substring(0, idx);

                if (type.GetGenericTypeDefinition() == typeof(Nullable<>))
                    return $"{GetTypeName(genericArgs[0])}?";
                
                return $"{baseTypeName}<{genericArgsStr}>";
            }

            if (type.IsNested && !type.IsNestedPublic)
                return $"{GetTypeName(type.DeclaringType)}.{type.Name}";

            return type.Name;
        }

        public static string GetMethodSignature(MethodInfo method)
        {
            var sb = new System.Text.StringBuilder();

            if (method.IsPublic)
                sb.Append("public ");
            else if (method.IsPrivate)
                sb.Append("private ");
            else if (method.IsFamily)
                sb.Append("protected ");
            else if (method.IsFamilyOrAssembly)
                sb.Append("protected internal ");
            else if (method.IsAssembly)
                sb.Append("internal ");

            if (method.IsStatic)
                sb.Append("static ");

            if (method.IsVirtual && !method.IsFinal && !method.IsAbstract)
                sb.Append("virtual ");
            else if (method.IsVirtual && method.IsFinal && !method.IsAbstract)
                sb.Append("override ");
            else if (method.IsAbstract)
                sb.Append("abstract ");

            sb.Append(GetTypeName(method.ReturnType));
            sb.Append(" ");
            sb.Append(method.Name);

            if (method.IsGenericMethod)
            {
                sb.Append("<");
                Type[] genericArgs = method.GetGenericArguments();
                sb.Append(string.Join(", ", genericArgs.Select(t => t.Name)));
                sb.Append(">");
            }

            sb.Append("(");
            var parameters = method.GetParameters();
            for (int i = 0; i < parameters.Length; i++)
            {
                var param = parameters[i];

                if (param.IsOut)
                    sb.Append("out ");
                else if (param.ParameterType.IsByRef)
                    sb.Append("ref ");

                sb.Append(GetTypeName(param.ParameterType.IsByRef ? param.ParameterType.GetElementType() : param.ParameterType));
                sb.Append(" ");
                sb.Append(param.Name);

                if (i < parameters.Length - 1)
                    sb.Append(", ");
            }
            sb.Append(")");

            return sb.ToString();
        }

        public static string GetFieldSignature(FieldInfo field)
        {
            var sb = new System.Text.StringBuilder();

            if (field.IsPublic)
                sb.Append("public ");
            else if (field.IsPrivate)
                sb.Append("private ");
            else if (field.IsFamily)
                sb.Append("protected ");
            else if (field.IsFamilyOrAssembly)
                sb.Append("protected internal ");
            else if (field.IsAssembly)
                sb.Append("internal ");

            if (field.IsStatic)
                sb.Append("static ");

            if (field.IsInitOnly)
                sb.Append("readonly ");

            if (field.IsLiteral && !field.IsInitOnly)
                sb.Append("const ");

            sb.Append(GetTypeName(field.FieldType));
            sb.Append(" ");
            sb.Append(field.Name);

            return sb.ToString();
        }

        public static bool IsSystemType(Type type)
        {
            string fullName = type.FullName ?? "";

            string[] excludedNamespaces = new[]
            {
                "System.",
                "Microsoft.",
                "UnityEngine.",
                "Unity.",
                "TMPro.",
                "Mono.",
                "AOT.",
                "JetBrains."
            };

            if (excludedNamespaces.Any(ns => fullName.StartsWith(ns)))
                return true;

            if (type.IsDefined(typeof(System.Runtime.CompilerServices.CompilerGeneratedAttribute), false))
                return true;

            if (type.Name.Contains("<") ||
                type.Name.Contains(">") ||
                type.Name.StartsWith("__") ||
                type.Name.Contains("AnonymousType") ||
                type.Name.Contains("DisplayClass") ||
                type.Name.Contains("<>"))
            {
                return true;
            }

            return false;
        }

        public static bool IsAutoGeneratedMethod(MethodInfo method)
        {
            if (method.IsDefined(typeof(System.Runtime.CompilerServices.CompilerGeneratedAttribute), false))
                return true;

            string methodName = method.Name;
            if (methodName.StartsWith("get_") || methodName.StartsWith("set_") ||
                methodName.StartsWith("add_") || methodName.StartsWith("remove_") ||
                methodName.Contains("<") || methodName.Contains(">") ||
                methodName.StartsWith("__") || methodName.StartsWith("<>"))
            {
                return true;
            }

            return false;
        }

        public static bool IsAutoGeneratedField(FieldInfo field)
        {
            if (field.IsDefined(typeof(System.Runtime.CompilerServices.CompilerGeneratedAttribute), false))
                return true;

            string fieldName = field.Name;
            if (fieldName.Contains("<") || fieldName.Contains(">") ||
                fieldName.StartsWith("__") || fieldName.StartsWith("<>"))
            {
                return true;
            }

            return false;
        }
    }
}
