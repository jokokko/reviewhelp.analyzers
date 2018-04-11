using System;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;
using static ReviewHelp.Analyzers.Infrastructure.Formats;

namespace ReviewHelp.Analyzers.Usage
{
	[DiagnosticAnalyzer(LanguageNames.CSharp)]
	public sealed class CtorEnumerableUsageAnalyzer : DiagnosticAnalyzer
	{
		public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
			ImmutableArray.Create(Descriptors.RH1005CtorEnumerableArgCouldBeMaterialized);

		public override void Initialize(AnalysisContext context)
		{
			context.EnableConcurrentExecution();
			context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);

			context.RegisterOperationBlockStartAction(ctx =>
			{
				if (!(ctx.OwningSymbol is IMethodSymbol m) || m.MethodKind != MethodKind.Constructor)
				{
					return;
				}

				ctx.RegisterOperationAction(oc => AnalyzeOperation(oc, m), OperationKind.SimpleAssignment);
			});
		}

		private void AnalyzeOperation(OperationAnalysisContext context, IMethodSymbol methodSymbol)
		{
			if (!(context.Operation is IAssignmentOperation operation))
			{
				return;
			}

			var semanticModel = context.Compilation.GetSemanticModel(operation.Syntax.SyntaxTree);

			if (!(operation.Value.Syntax is IdentifierNameSyntax identifier))
			{
				return;
			}

			var sourceSymbol = semanticModel.GetSymbolInfo(identifier);

			if (sourceSymbol.Symbol == null)
			{
				return;
			}

			var matchingParameter = methodSymbol.Parameters.FirstOrDefault(x => sourceSymbol.Symbol.Name.Equals(x.Name, StringComparison.Ordinal));

			if (matchingParameter == null)
			{
				return;
			}

			bool IsIEnumerable(ISymbol sym)
			{
				return sym.ToDisplayString(FqfWithoutGenericType).Equals("System.Collections.Generic.IEnumerable");
			}

			if (!IsIEnumerable(matchingParameter.Type))
			{
				return;
			}

			var targetSymbol = semanticModel.GetSymbolInfo(operation.Target.Syntax);

			if (targetSymbol.Symbol == null)
			{
				return;
			}

			var targetType = semanticModel.GetTypeInfo(operation.Target.Syntax);

			if (targetType.Type == null || !IsIEnumerable(targetType.Type))
			{
				return;
			}

			context.ReportDiagnostic(Diagnostic.Create(
				SupportedDiagnostics[0],
				operation.Syntax.GetLocation(), targetSymbol.Symbol.Name, matchingParameter.Name,
				SymbolDisplayFormat.CSharpShortErrorMessageFormat.WithParameterOptions(
					SymbolDisplayParameterOptions.None)));
		}
	}
}