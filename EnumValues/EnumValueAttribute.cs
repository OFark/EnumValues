using System;

namespace EnumValues
{
    [AttributeUsage(AttributeTargets.Field)]
    public class EnumValueAttribute : Attribute
    {
        private readonly Type valueType;

        public EnumValueAttribute(Type? valueType = null)
        {
            this.valueType = valueType ?? typeof(object);
        }
    }
}
