using System;
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

namespace RegexAnalyzer
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SetUpCodeFixProvider)), Shared]
    public class SetUpCodeFixProvider : CodeFixProvider
    {
        private const string Title = "Make public";

        private readonly SyntaxToken _whitespaceToken = SyntaxFactory.Token(SyntaxTriviaList.Create(SyntaxFactory.Space), SyntaxKind.StringLiteralToken, SyntaxTriviaList.Empty);

        public override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(SetUpAttributeAnalyzer.DiagnosticId); }
        }

        public sealed override FixAllProvider GetFixAllProvider()
        {
            // See https://github.com/dotnet/roslyn/blob/master/docs/analyzers/FixAllProvider.md for more information on Fix All Providers
            return WellKnownFixAllProviders.BatchFixer;
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            // TODO: Replace the following code with your own analysis, generating a CodeAction for each fix to suggest
            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            // Find the type declaration identified by the diagnostic.
            var declaration = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<MethodDeclarationSyntax>().First();

            // Register a code action that will invoke the fix.
            context.RegisterCodeFix(
                CodeAction.Create(Title, 
                                  c => ReplacePropertyModifierAsync(context.Document, declaration, SyntaxKind.PublicKeyword, c),
                                  Title),
                diagnostic);
        }

        private async Task<Document> ReplacePropertyModifierAsync(Document document, MethodDeclarationSyntax method, SyntaxKind methodModifier, CancellationToken cancellationToken)
        {
            var previousWhiteSpacesToken = SyntaxFactory.Token(method.GetLeadingTrivia(), SyntaxKind.StringLiteralToken, SyntaxTriviaList.Empty);

            var newProperty = method.WithModifiers(SyntaxTokenList.Create(previousWhiteSpacesToken)
            .Add(SyntaxFactory.Token(methodModifier))
            .Add(_whitespaceToken));

            if (method.Modifiers.Any(m => m.Kind() == SyntaxKind.VirtualKeyword))
            {
                newProperty = newProperty.AddModifiers(SyntaxFactory.Token(SyntaxKind.VirtualKeyword), _whitespaceToken);
            }

            return await ReplacePropertyInDocumentAsync(document, method, newProperty, cancellationToken);
        }

        private static async Task<Document> ReplacePropertyInDocumentAsync(Document document, MethodDeclarationSyntax method, MethodDeclarationSyntax newMethod, CancellationToken cancellationToken)
        {
            var root = await document.GetSyntaxRootAsync(cancellationToken);
            var newRoot = root.ReplaceNode(method, new[] { newMethod });

            return document.WithSyntaxRoot(newRoot);
        }
    }
}
