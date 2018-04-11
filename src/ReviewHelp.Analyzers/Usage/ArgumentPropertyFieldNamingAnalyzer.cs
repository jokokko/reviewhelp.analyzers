using System;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;

namespace ReviewHelp.Analyzers.Usage
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class ArgumentPropertyFieldNamingAnalyzer : DiagnosticAnalyzer
	{
	    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
	        ImmutableArray.Create(Descriptors.RH1004ArgumentNameDiffersFromPropertyAssignedTo);

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
		    var sourceSymbol = semanticModel.GetSymbolInfo(operation.Value.Syntax);

		    if (sourceSymbol.Symbol == null)
		    {
		        return;
		    }

            var matchingParameter = methodSymbol.Parameters.FirstOrDefault(x => sourceSymbol.Symbol.Name.Equals(x.Name, StringComparison.Ordinal));

		    if (matchingParameter == null)
		    {
		        return;
		    }

		    var targetSymbol = semanticModel.GetSymbolInfo(operation.Target.Syntax);

		    if (targetSymbol.Symbol == null)
		    {
		        return;
		    }

		    if (!targetSymbol.Symbol.Name.Equals(sourceSymbol.Symbol.Name, StringComparison.OrdinalIgnoreCase))
		    {
				context.ReportDiagnostic(Diagnostic.Create(
					SupportedDiagnostics[0],
					operation.Syntax.GetLocation(), matchingParameter.Name, targetSymbol.Symbol.Name,
						SymbolDisplayFormat.CSharpShortErrorMessageFormat.WithParameterOptions(
							SymbolDisplayParameterOptions.None)));
			}
		
		}
	}
}