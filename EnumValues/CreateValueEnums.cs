using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EnumValues
{
    [Generator]
    public class CreateValueEnums : ISourceGenerator
    {
        public void Execute(GeneratorExecutionContext context)
        {
            if (context.SyntaxContextReceiver is not SyntaxReceiver receiver) return;

            foreach (var r in receiver.EnumReceivers)
            {
                StringBuilder source = new($$"""
                    #nullable enable
                    namespace {{r.Namespace}}
                    {
                        {{r.Accessibility}} partial struct {{r.WrapperClassName}}
                        {
                            private {{r.EnumName}} {{r.EnumVariableName}};

                            private Dictionary<{{r.EnumName}}, object?> values = new();
                        
                            public {{r.WrapperClassName}}({{r.EnumName}} {{r.EnumVariableName}}, object? value = null)
                            {
                                this.{{r.EnumVariableName}} = {{r.EnumVariableName}};
                                this.values[{{r.EnumVariableName}}] = value;
                            }
                        
                            public static {{r.WrapperClassName}} operator |({{r.WrapperClassName}} first, {{r.WrapperClassName}} second)
                            {
                                var response = new {{r.WrapperClassName}}(first.{{r.EnumVariableName}} | second.{{r.EnumVariableName}});
                                response.values = first.values.Concat(second.values).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
                                return response;
                            }
                        
                            public object? this[{{r.EnumName}} key]
                            {
                                get { return values[key]; }
                                set { values[key] = value; }
                            }
                        
                            public bool HasFlag({{r.EnumName}} {{r.EnumVariableName}}) => this.{{r.EnumVariableName}}.HasFlag({{r.EnumVariableName}});

                            {{string.Join("\r\n        ", r.Members.Where(kvp => kvp.Value is not null).Select(kvp => $"public {kvp.Value} {kvp.Key} => ({kvp.Value})values[{r.EnumName}.{kvp.Key}]!;"))}}
                        }

                        {{r.Accessibility}} partial struct {{r.StaticClassName}}
                        {
                            {{ string.Join("\r\n        ", r.Members.Select(kvp => $"public static {r.WrapperClassName} {kvp.Key}{(kvp.Value is not null ? $"({kvp.Value} value)" : "" )} {(kvp.Value is not null ? "=>" : "=")} new({r.EnumName}.{kvp.Key}{(kvp.Value is not null ? $", value" : "")});"))}}
                        }
                    }
                    """);
                context.AddSource($"{r.EnumName}_values.g.cs", SourceText.From(source.ToString(), Encoding.UTF8));
            }
        }

        public void Initialize(GeneratorInitializationContext context) => context.RegisterForSyntaxNotifications(() => new SyntaxReceiver());

    }

    file class SyntaxReceiver : ISyntaxContextReceiver
    {
        public List<EnumReceiver> EnumReceivers { get; } = new List<EnumReceiver>();

        public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
        {
            if (context.Node is EnumDeclarationSyntax enumDeclarationSyntax)
            {
                var valueFlagsAttributeName = nameof(ValueFlagsAttribute).Replace("Attribute", "");

                var attrs = enumDeclarationSyntax.AttributeLists
                                            .SelectMany(x => x.Attributes)
                                            .Where(attr => attr.Name.GetText().ToString() == valueFlagsAttributeName);
                if (attrs.Any())
                {
                    EnumReceivers.Add(new EnumReceiver(enumDeclarationSyntax, context.SemanticModel, attrs.FirstOrDefault()));
                }
            }
        }
    }

    file class EnumReceiver
    {
        public EnumReceiver(EnumDeclarationSyntax enumDeclarationSyntax, SemanticModel semanticModel, AttributeSyntax attributeSyntax)
        {
            EnumName = enumDeclarationSyntax.Identifier.Text;
            var parent = enumDeclarationSyntax.Parent;

            Accessibility = semanticModel.GetDeclaredSymbol(enumDeclarationSyntax)?.DeclaredAccessibility.ToString().ToLower() ?? "";

            while (parent is ClassDeclarationSyntax classDeclaration)
            {
                EnumName = $"{classDeclaration.Identifier.Text}.{EnumName}";
                parent = classDeclaration.Parent;
                Accessibility = semanticModel.GetDeclaredSymbol(classDeclaration)?.DeclaredAccessibility.ToString().ToLower() ?? "";
            }

            if (parent is NamespaceDeclarationSyntax namespaceDeclaration)
            {
                Namespace = namespaceDeclaration.Name.ToString();
            }

            if (parent is FileScopedNamespaceDeclarationSyntax fileScopedNamespaceDeclarationSyntax)
            {
                Namespace = fileScopedNamespaceDeclarationSyntax.Name.ToString();
            }

            var wrapperNameArgExp = attributeSyntax.ArgumentList?.Arguments.FirstOrDefault(x => x.NameColon?.Name.Identifier.Text == "valueTypeName")?.Expression as LiteralExpressionSyntax;
            var staticNameArgExp = attributeSyntax.ArgumentList?.Arguments.FirstOrDefault(x => x.NameColon?.Name.Identifier.Text == "valuedEnumName")?.Expression as LiteralExpressionSyntax;

            EnumVariableName = EnumName.ToLower().Equals(EnumName) ? $"{EnumName}Var" : EnumName.ToLower();
            WrapperClassName = wrapperNameArgExp?.NormalizeWhitespace().Token.Value as string ?? $"{EnumName}Value";
            StaticClassName = staticNameArgExp?.NormalizeWhitespace().Token.Value as string ?? $"{EnumName}Values";

            var enumValueAttributeName = nameof(EnumValueAttribute).Replace("Attribute", "");

            foreach (var x in enumDeclarationSyntax.Members)
            {
                var enumValAttr = x.AttributeLists.SelectMany(x => x.Attributes)
                                                  .FirstOrDefault(attr => attr.Name.GetText().ToString() == enumValueAttributeName);
                var typenameArgExp = enumValAttr?.ArgumentList?.Arguments.FirstOrDefault(x => x.Expression is TypeOfExpressionSyntax).Expression as TypeOfExpressionSyntax;

                string? typename = null;
                if (typenameArgExp is not null)
                {
                    var symbolInfo = semanticModel.GetSymbolInfo(typenameArgExp.Type);
                    var symbolInfoSymbol = symbolInfo.Symbol;

                    typename = symbolInfoSymbol?.OriginalDefinition.ToString();
                }


                Members.Add(x.Identifier.Text, typename);
            }
        }

        public string EnumName { get; }
        public string EnumVariableName { get; }
        public string WrapperClassName { get; }
        public string StaticClassName { get; }
        public string? Namespace { get; }

        public Dictionary<string, string?> Members { get; } = new();

        public string Accessibility { get; private set; }
    }
}
