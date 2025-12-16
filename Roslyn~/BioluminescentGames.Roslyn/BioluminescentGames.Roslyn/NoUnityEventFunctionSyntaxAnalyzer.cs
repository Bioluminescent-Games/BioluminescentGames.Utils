using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BioluminescentGames.Roslyn;

/// <summary>
/// A sample analyzer that reports the company name being used in class declarations.
/// Traverses through the Syntax Tree and checks the name (identifier) of each class node.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class NoUnityEventFunctionSyntaxAnalyzer : DiagnosticAnalyzer
{
    public readonly string[] DisallowedUnityEventFunctions = ["Update"];
    public readonly string[] IncludedAssemblies = ["BioluminescentGames", "BackroomsGame", "PuzzleGame"];

    // Preferred format of DiagnosticId is Your Prefix + Number, e.g. CA1234.
    public const string DIAGNOSTIC_ID = "BG0001";

    private static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.BG0001Title),
        Resources.ResourceManager, typeof(Resources));
    private static readonly LocalizableString MessageFormat =
        new LocalizableResourceString(nameof(Resources.BG0001MessageFormat), Resources.ResourceManager,
            typeof(Resources));
    private static readonly LocalizableString Description =
        new LocalizableResourceString(nameof(Resources.BG0001Description), Resources.ResourceManager,
            typeof(Resources));

    private const string k_Category = "Performance";

    public static readonly DiagnosticDescriptor Rule = new(DIAGNOSTIC_ID, Title, MessageFormat, k_Category,
        DiagnosticSeverity.Error, isEnabledByDefault: true, description: Description);

    // Keep in mind: you have to list your rules here.
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
        ImmutableArray.Create(Rule);

    public override void Initialize(AnalysisContext context)
    {
        // You must call this method to avoid analyzing generated code.
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);

        // You must call this method to enable the Concurrent Execution.
        context.EnableConcurrentExecution();

        // Subscribe to the Syntax Node with the appropriate 'SyntaxKind' (ClassDeclaration) action.
        // To figure out which Syntax Nodes you should choose, consider installing the Roslyn syntax tree viewer plugin Rossynt: https://plugins.jetbrains.com/plugin/16902-rossynt/
        context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.MethodDeclaration);

        // Check other 'context.Register...' methods that might be helpful for your purposes.
    }

    /// <summary>
    /// Executed for each Syntax Node with 'SyntaxKind' is 'ClassDeclaration'.
    /// </summary>
    /// <param name="context">Operation context.</param>
    private void AnalyzeNode(SyntaxNodeAnalysisContext context)
    {
        // The Roslyn architecture is based on inheritance.
        // To get the required metadata, we should match the 'Node' object to the particular type: 'ClassDeclarationSyntax'.
        if (context.Node is not MethodDeclarationSyntax methodDeclarationNode)
            return;

        // 'Identifier' means the token of the node. In this case, the identifier of the 'ClassDeclarationNode' is the class name.
        SyntaxToken methodDeclarationIdentifier = methodDeclarationNode.Identifier;

        // Find class symbols whose name contains the company name.
        if (!DisallowedUnityEventFunctions.Contains(methodDeclarationIdentifier.ValueText))
            return;

        ClassDeclarationSyntax? containingClass = methodDeclarationNode.Ancestors().OfType<ClassDeclarationSyntax>().FirstOrDefault();
        if (containingClass == null)
            return;

        INamedTypeSymbol? classSymbol = context.SemanticModel.GetDeclaredSymbol(containingClass);
        if (classSymbol == null)
            return;

        if (!IsInIncludedAssembly(classSymbol))
            return;

        if (!DoesInheritFrom(classSymbol, "UnityEngine.MonoBehaviour"))
            return;

        context.ReportDiagnostic(Diagnostic.Create(Rule,
            methodDeclarationIdentifier.GetLocation(),
            classSymbol.Name,
            methodDeclarationIdentifier.ValueText
        ));
    }

    private bool IsInIncludedAssembly(INamedTypeSymbol classSymbol) =>
        IncludedAssemblies.Any(assembly => classSymbol.ContainingAssembly.Name.Contains(assembly));

    private static bool DoesInheritFrom(INamedTypeSymbol? classSymbol, string fullyQualifiedBaseTypeName)
    {
        INamedTypeSymbol? currentBase = classSymbol?.BaseType;

        while (currentBase != null)
        {
            if (currentBase.ToDisplayString() == fullyQualifiedBaseTypeName)
            {
                return true;
            }
            currentBase = currentBase.BaseType;
        }
        return false;
    }
}
