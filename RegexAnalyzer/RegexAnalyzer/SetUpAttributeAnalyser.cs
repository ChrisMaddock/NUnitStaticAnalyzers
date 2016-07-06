using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using NUnit.Framework;

namespace RegexAnalyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SetUpAttributeAnalyzer : DiagnosticAnalyzer
    {
        internal const string DiagnosticId = "SetUpAttributeAnalyzer";
        private const string Title = "SetUpAttribute has errors";
        private const string MessageFormat = "Method '{0}' with SetUpAttribute must be public";
        private const string Description = "Methods with SetUpAttribute must be public";
        private const string Category = "Naming";

        private static string AttributeName = typeof(SetUpAttribute).Name;

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat,
            Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get { return ImmutableArray.Create(Rule); }
        }

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSymbolAction(AnalyzeSymbol, SymbolKind.Method);
        }

        private static void AnalyzeSymbol(SymbolAnalysisContext context)
        {
            var symbol = context.Symbol;
            var attributes = symbol.GetAttributes();

            if (!attributes.Any(a => a.AttributeClass.Name == AttributeName)) return;
            if (symbol.DeclaredAccessibility != Accessibility.Private) return;

            var diagnostic = Diagnostic.Create(Rule, symbol.Locations[0], symbol.Name);
            context.ReportDiagnostic(diagnostic);
        }
    }
}
