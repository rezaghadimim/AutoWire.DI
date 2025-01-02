using System.Collections.Immutable;
using System.Diagnostics;
using System.Text.Json;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace AutoWire.DI.Analyzers.CodeAnalysis;

/// <summary>
/// Analyzes the usage of the <c>AutoInjectAttribute</c> on class declarations.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class AutoInjectAnalyzer : DiagnosticAnalyzer
{
    private const string DiagnosticId = "AutoInjectUsage";
    private const string Title = "AutoInject Attribute Usage";
    private const string MessageFormat = "The class '{0}' is not annotated with AutoInject but may be intended for dependency injection";
    private const string Description = "Classes intended for dependency injection should be annotated with AutoInject.";
    private const string Category = "Usage";

    /// <summary>
    /// Descriptor for the diagnostic rule that checks for the usage of the <c>AutoInjectAttribute</c>.
    /// </summary>
    private static readonly DiagnosticDescriptor Rule = new(
        DiagnosticId,
        Title,
        MessageFormat,
        Category,
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: Description
    );

    /// <inheritdoc />
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => [Rule];

    /// <inheritdoc />
    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
        context.EnableConcurrentExecution();
        context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.ClassDeclaration);
    }

    /// <summary>
    /// Analyzes class declarations to detect the presence of the <c>AutoInjectAttribute</c>.
    /// </summary>
    /// <param name="context">The syntax node analysis context.</param>
    private static void AnalyzeNode(SyntaxNodeAnalysisContext context)
    {
        var classDeclaration = (ClassDeclarationSyntax)context.Node;
        var symbol = ModelExtensions.GetDeclaredSymbol(context.SemanticModel, classDeclaration);

        if (symbol is null)
        {
            return;
        }

        // Check for the presence of AutoInject attribute
        var hasAutoInjectAttribute = symbol.GetAttributes().Any(attr => attr.AttributeClass?.Name == "AutoInject");
        Console.WriteLine($"Analyzing class {JsonSerializer.Serialize(symbol.GetAttributes().Select(x => x.AttributeClass?.Name ?? "withoutname").ToList())}");
        Console.WriteLine($"Class {classDeclaration.Identifier} with AutoInject: {hasAutoInjectAttribute}");

        if (hasAutoInjectAttribute)
        {
            Console.WriteLine("Ignore class with AutoInject attribute");
            return;
        }

        context.ReportDiagnostic(Diagnostic.Create(Rule, classDeclaration.GetLocation(), classDeclaration.Identifier.Text));
    }
}
