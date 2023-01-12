using EnumValues;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Diagnostics;
using System.Reflection;
using TryIt;

var filtersFlags = Filters.Incomplete | Filters.Tenable(3);


if (filtersFlags.HasFlag(Filter.None)) Console.WriteLine("None");
if (filtersFlags.HasFlag(Filter.Incomplete)) Console.WriteLine("All Incomplete");
if (filtersFlags.HasFlag(Filter.Tenable)) Console.WriteLine($"Get emails where attempts < {filtersFlags.Tenable}");

return;


//var requiredFiles = new[]
//        {
//            typeof(object).Assembly.Location,
//            typeof(System.Linq.Expressions.BinaryExpression).Assembly.Location
//        };

//var metaDataReferences = requiredFiles.Select(x => AssemblyMetadata.CreateFromFile(Path.Combine(Environment.CurrentDirectory, x)).GetReference()).ToList();

//// Create the 'input' compilation that the generator will act on
//var inputCompilation = CreateCompilation("""
//namespace TryIt
//{
//    [ValueFlags(valueTypeName: "FilterWithValues", valuedEnumName: "Filters")]
//    internal enum Filter
//    {
//        None = 1,
//        Incomplete = 1 << 1,
//        [EnumValue(typeof(int))]
//        Tenable = 1 << 2,
//        [EnumValue(typeof(string))]
//        ByName = 1 << 3,
//    }
//}

//"""
//).WithReferences(metaDataReferences);


//// directly create an instance of the generator
//// (Note: in the compiler this is loaded from an assembly, and created via reflection at runtime)
//var generator = new CreateValueEnums();
 
//// Create the driver that will control the generation, passing in our generator
//GeneratorDriver driver = CSharpGeneratorDriver.Create(generator);

//// Run the generation pass
//// (Note: the generator driver itself is immutable, and all calls return an updated version of the driver that you should use for subsequent calls)
//driver = driver.RunGeneratorsAndUpdateCompilation(inputCompilation, out var outputCompilation, out var diagnostics);

//// We can now assert things about the resulting compilation:
//Debug.Assert(diagnostics.IsEmpty); // there were no diagnostics created by the generators
//Debug.Assert(outputCompilation.SyntaxTrees.Count() == 2); // we have two syntax trees, the original 'user' provided one, and the one added by the generator
//                                                          //Debug.Assert(outputCompilation.GetDiagnostics().IsEmpty, string.Join(", ", outputCompilation.GetDiagnostics().Select(d => d.GetMessage()))); // verify the compilation with the added source has no diagnostics

//// Or we can look at the results directly:
//var runResult = driver.GetRunResult();

//// The runResult contains the combined results of all generators passed to the driver
//Debug.Assert(runResult.GeneratedTrees.Length == 1);
//Debug.Assert(runResult.Diagnostics.IsEmpty);

//// Or you can access the individual results on a by-generator basis
//var generatorResult = runResult.Results[0];
//Debug.Assert(generatorResult.Generator == generator);
//Debug.Assert(generatorResult.Diagnostics.IsEmpty);
//Debug.Assert(generatorResult.GeneratedSources.Length == 1);
//Debug.Assert(generatorResult.Exception is null);

//static Compilation CreateCompilation(string source)
//        => CSharpCompilation.Create("compilation",
//            new[] { CSharpSyntaxTree.ParseText(source) },
//            new[] { MetadataReference.CreateFromFile(typeof(Binder).GetTypeInfo().Assembly.Location) },
//            new CSharpCompilationOptions(OutputKind.ConsoleApplication));