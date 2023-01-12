using System;

namespace EnumValues
{
    [AttributeUsage(AttributeTargets.Enum)]
    public class ValueFlagsAttribute : FlagsAttribute
    {
        public readonly string? ValuedEnumName;

        public readonly string? ValueTypeName;

        public ValueFlagsAttribute(string? valueTypeName = null, string? valuedEnumName = null)
        {
            ValueTypeName = valueTypeName;
            ValuedEnumName = valuedEnumName;
        }
    }
}
