using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using SyntaxFactory = Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using SyntaxKind = Microsoft.CodeAnalysis.CSharp.SyntaxKind;

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

        const string requiredUsing = "BioluminescentGames.Utils.MonoBehaviourExtensions";
        bool hasUsing = updatedRoot.Usings.Any(u => u.Name.ToString() == requiredUsing);
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
