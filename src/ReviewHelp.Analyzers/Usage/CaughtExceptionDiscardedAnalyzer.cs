using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;

namespace ReviewHelp.Analyzers.Usage
{
	[DiagnosticAnalyzer(LanguageNames.CSharp)]
	public sealed class CaughtExceptionDiscardedAnalyzer : DiagnosticAnalyzer
	{
		public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
			ImmutableArray.Create(Descriptors.RH1002CaughtExceptionDiscarded, Descriptors.RH1003CaughtExceptionNotCapturedDiscarded);

		public override void Initialize(AnalysisContext context)
		{
			context.EnableConcurrentExecution();
			context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);

			context.RegisterOperationAction(
				AnalyzeOperation,
				OperationKind.CatchClause
			);
		}
		
		private void AnalyzeOperation(OperationAnalysisContext context)
		{
			var syntax = (ICatchClauseOperation) context.Operation;

			var variableDeclaration = syntax.ExceptionDeclarationOrExpression as IVariableDeclaratorOperation;
			
			var throwStatements = syntax.Handler.Syntax.DescendantNodes().OfType<ThrowStatementSyntax>().ToArray();

			var model = context.Compilation.GetSemanticModel(context.Operation.Syntax.SyntaxTree);

			foreach (var throwStatement in throwStatements)
			{
				if (context.CancellationToken.IsCancellationRequested)
				{
					return;
				}

				// ReSharper disable once InvertIf
				if (throwStatement.Expression is ObjectCreationExpressionSyntax objectCreation)
				{
					if (variableDeclaration != null)
					{
						if (!objectCreation.ArgumentList.Arguments.Any(x =>
						{
							var thrownException = model.GetSymbolInfo(x.Expression);
							return thrownException.Symbol != null && thrownException.Symbol.Equals(variableDeclaration.Symbol);
						}))
						{
							context.ReportDiagnostic(Diagnostic.Create(
								SupportedDiagnostics[0],
								throwStatement.Expression.GetLocation(), SymbolDisplay.ToDisplayString(variableDeclaration.Symbol,
									SymbolDisplayFormat.CSharpShortErrorMessageFormat.WithParameterOptions(
										SymbolDisplayParameterOptions.None))));
						}
					}
					else
					{
						context.ReportDiagnostic(Diagnostic.Create(
							SupportedDiagnostics[1],
							throwStatement.Expression.GetLocation()));
					}					
				}
			}
		}
	}
}