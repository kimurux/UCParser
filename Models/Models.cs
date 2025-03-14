using System.Collections.Generic;

namespace UCParser.Models
{
    public class MethodInfoData
    {
        public string Name { get; set; }
        public int Token { get; set; }
        public string ReturnType { get; set; }
        public List<ParameterInfoData> Parameters { get; set; } = new List<ParameterInfoData>();
        public string DeclaringType { get; set; }
        public bool IsPublic { get; set; }
        public bool IsPrivate { get; set; }
        public bool IsStatic { get; set; }
        public bool IsVirtual { get; set; }
        public bool IsAbstract { get; set; }
        public bool IsOverride { get; set; }
    }

    public class ParameterInfoData
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public bool IsOut { get; set; }
        public bool IsRef { get; set; }
        public bool IsOptional { get; set; }
    }

    public class FieldInfoData
    {
        public string Name { get; set; }
        public int Token { get; set; }
        public string Type { get; set; }
        public string DeclaringType { get; set; }
        public bool IsPublic { get; set; }
        public bool IsPrivate { get; set; }
        public bool IsStatic { get; set; }
        public bool IsReadOnly { get; set; }
        public bool IsConstant { get; set; }
    }

    public class TypeInfoData
    {
        public string Name { get; set; }
        public string FullName { get; set; }
        public int Token { get; set; }
        public bool IsPublic { get; set; }
        public bool IsClass { get; set; }
        public bool IsInterface { get; set; }
        public bool IsEnum { get; set; }
        public bool IsStruct { get; set; }
        public List<MethodInfoData> Methods { get; set; } = new List<MethodInfoData>();
        public List<FieldInfoData> Fields { get; set; } = new List<FieldInfoData>();
    }
}