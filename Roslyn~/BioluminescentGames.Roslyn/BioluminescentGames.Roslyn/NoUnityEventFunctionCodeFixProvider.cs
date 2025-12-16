using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace BioluminescentGames.Roslyn;

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(NoUnityEventFunctionCodeFixProvider)), Shared]
public class NoUnityEventFunctionCodeFixProvider : CodeFixProvider
{
    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        Diagnostic diagnostic = context.Diagnostics.Single();

        TextSpan diagnosticSpan = diagnostic.Location.SourceSpan;

        SyntaxNode? root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

        SyntaxNode? diagnosticNode = root?.FindNode(diagnosticSpan);

        if (diagnosticNode is not MethodDeclarationSyntax declaration)
            return;

        context.RegisterCodeFix(
            CodeAction.Create(
                Resources.BG0001CodeFixTitle,
                createChangedSolution: c => UseCustomUpdateSystemAsync(context.Document, declaration, c),
                equivalenceKey: nameof(Resources.BG0001CodeFixTitle)
            ),
            diagnostic
        );
    }

    public override ImmutableArray<string> FixableDiagnosticIds { get; } =
        ImmutableArray.Create(NoUnityEventFunctionSyntaxAnalyzer.DIAGNOSTIC_ID);

    private async Task<Solution> UseCustomUpdateSystemAsync(Document contextDocument, MethodDeclarationSyntax? declaration, CancellationToken cancellationToken)
    {
        string newName = "On" + declaration!.Identifier.Text;
        SemanticModel? semanticModel = await contextDocument.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);
        ISymbol? typeSymbol = semanticModel?.GetDeclaredSymbol(declaration, cancellationToken);
        if (typeSymbol == null) return contextDocument.Project.Solution;

        SyntaxNode? root = await contextDocument.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
        if (root is not CompilationUnitSyntax compilationUnit)
            return contextDocument.Project.Solution;

        MethodDeclarationSyntax updatedMethod =
            declaration
                .WithIdentifier(SyntaxFactory.Identifier(newName))
                .WithReturnType(SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.VoidKeyword)))
                .WithModifiers(
                    SyntaxFactory.TokenList(
                        SyntaxFactory.Token(SyntaxKind.PublicKeyword),
                        SyntaxFactory.Token(SyntaxKind.OverrideKeyword)
                    )
                );

        CompilationUnitSyntax updatedRoot = compilationUnit.ReplaceNode(declaration, updatedMethod);

        // Update the containing class base type:
        // - MonoBehaviour => BioluminescentBehaviour
        // - MonoSingleton<TSelf> => BioluminescentSingleton<TSelf>
        ClassDeclarationSyntax? containingClass = declaration.FirstAncestorOrSelf<ClassDeclarationSyntax>();
        if (containingClass is not null && semanticModel is not null)
        {
            ClassDeclarationSyntax updatedClass = containingClass;

            if (containingClass.BaseList is not null)
            {
                SeparatedSyntaxList<BaseTypeSyntax> bases = containingClass.BaseList.Types;

                for (int i = 0; i < bases.Count; i++)
                {
                    BaseTypeSyntax baseType = bases[i];
                    TypeSyntax baseTypeSyntax = baseType.Type;

                    ITypeSymbol? baseTypeSymbol = semanticModel.GetTypeInfo(baseTypeSyntax, cancellationToken).Type;

                    switch (baseTypeSymbol)
                    {
                        // Match: MonoSingleton<TSelf>
                        case INamedTypeSymbol
                        {
                            Name: "MonoSingleton", IsGenericType: true, Arity: 1
                        } monoSingletonSymbol:
                        {
                            string tSelf = monoSingletonSymbol.TypeArguments[0].ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);

                            bases = bases.Replace(
                                baseType,
                                SyntaxFactory.SimpleBaseType(SyntaxFactory.ParseTypeName($"BioluminescentSingleton<{tSelf}>"))
                            );
                            break;
                        }
                        // Match: NetworkSingleton<TSelf>
                        case INamedTypeSymbol
                        {
                            Name: "NetworkSingleton", IsGenericType: true, Arity: 1
                        } networkSingletonSymbol:
                        {
                            string tSelf = networkSingletonSymbol.TypeArguments[0].ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);

                            bases = bases.Replace(
                                baseType,
                                SyntaxFactory.SimpleBaseType(SyntaxFactory.ParseTypeName($"BioluminescentNetworkSingleton<{tSelf}>"))
                            );
                            break;
                        }
                        // Match: NetworkBehaviour
                        case INamedTypeSymbol
                        {
                            Name: "NetworkBehaviour"
                        }:
                            bases = bases.Replace(
                                baseType,
                                SyntaxFactory.SimpleBaseType(SyntaxFactory.ParseTypeName("BioluminescentNetworkBehaviour"))
                            );
                            break;
                        // Match: UnityEngine.MonoBehaviour
                        case INamedTypeSymbol
                        {
                            Name: "MonoBehaviour"
                        }:
                            bases = bases.Replace(
                                baseType,
                                SyntaxFactory.SimpleBaseType(SyntaxFactory.ParseTypeName("BioluminescentBehaviour"))
                            );
                            break;
                    }
                }

                updatedClass = updatedClass.WithBaseList(containingClass.BaseList.WithTypes(bases));
            }

            updatedRoot = updatedRoot.ReplaceNode(containingClass, updatedClass);
        }

        const string requiredUsing = "BioluminescentGames.Utils.MonoBehaviourExtensions";
        bool hasUsing = updatedRoot.Usings.Any(u => u.Name?.ToString() == requiredUsing);
        if (!hasUsing)
        {
            UsingDirectiveSyntax usingDirective =
                SyntaxFactory.UsingDirective(SyntaxFactory.ParseName(requiredUsing));

            updatedRoot = updatedRoot.AddUsings(usingDirective);
        }

        Document updatedDocument = contextDocument.WithSyntaxRoot(updatedRoot);
        return updatedDocument.Project.Solution;
    }
}
