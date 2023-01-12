using EnumValues;

namespace TryIt
{
    [ValueFlags(valueTypeName: "FilterWithValues", valuedEnumName: "Filters")]
    internal enum Filter
    {
        None = 1,
        Incomplete = 1 << 1,
        [EnumValue(typeof(int))]
        Tenable = 1 << 2,
        [EnumValue(typeof(string))]
        ByName = 1 << 3,
    }
}
