# EnumValues #
EnumValues add values to support to enum flags

add to you enum class:
`[ValueFlags(valueTypeName: "FilterWithValues", valuedEnumName: "Filters")]`

Add to your enum values: 
`[EnumValue(typeof(int))]`

## Example ##
```c#
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
```


Then you can set flags with:
```c#
var filtersFlags = Filters.Incomplete | Filters.Tenable(3);
```
Notice the use of the "Filters" class that has been generated from the valuedEnumName property
The filtersFlags variable is a struct type called "FilterWithValues"  that has been generated from the valueTypeName property

Then you can test with:
```c#
if (filtersFlags.HasFlag(Filter.None)) Console.WriteLine("None");
if (filtersFlags.HasFlag(Filter.Incomplete)) Console.WriteLine("All Incomplete");
if (filtersFlags.HasFlag(Filter.Tenable)) Console.WriteLine($"Get emails where attempts < {filtersFlags.Tenable}");
```
