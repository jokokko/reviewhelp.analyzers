using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;

namespace ReviewHelp.Analyzers.Usage
{    
    public abstract class NullableTypeAnalyzer : DiagnosticAnalyzer
    {
#pragma warning disable RS1008 // Avoid storing per-compilation data into the fields of a diagnostic analyzer.
        private readonly Func<INamedTypeSymbol, bool> shouldReport;
#pragma warning restore RS1008 // Avoid storing per-compilation data into the fields of a diagnostic analyzer.
        
        public override void Initialize(AnalysisContext context)
        {
            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);

            context.RegisterOperationAction(
                AnalyzeOperation,                
                OperationKind.SimpleAssignment,                                
                OperationKind.FieldInitializer,
                OperationKind.PropertyInitializer,
                OperationKind.VariableInitializer
            );                        
        }

        // ReSharper disable once CyclomaticComplexity
        private void AnalyzeOperation(OperationAnalysisContext context)
        {
            IOperation t = null;
            SyntaxNode def = null;

            switch (context.Operation)
            {
                case ISymbolInitializerOperation op:
                {
                    t = op;
                    def = op.Value.Syntax;
                    break;
                }
                case IAssignmentOperation op:
                {
                    t = op.Target;
                    def = op.Value.Syntax;                                        
                    break;
                }
            }
                                   
            if (t == null || !(def is DefaultExpressionSyntax defaultExpression))
            {
                return;
            }
            
            switch (t)
            {
                case IVariableInitializerOperation symbolInit:
                {
                    if (symbolInit.Parent is IVariableDeclaratorOperation varDecrl && varDecrl.Symbol?.Type is INamedTypeSymbol symbol)
                    {                        
                        Check(context, defaultExpression, symbol);
                    }
                    break;
                }
                case IFieldReferenceOperation assignment:
                {
                    if (assignment.Field?.Type is INamedTypeSymbol symbol)
                    {
                        Check(context, defaultExpression, symbol);
                    }
                    break;
                }
                case IPropertyInitializerOperation propInitialize:
                {
                    foreach (var f in propInitialize.InitializedProperties)
                    {
                        if (f.Type is INamedTypeSymbol symbol)
                        {
                            Check(context, defaultExpression, symbol);
                        }
                    }
                    break;
                }
                case IFieldInitializerOperation fieldInitialize:
                    {
                        foreach (var f in fieldInitialize.InitializedFields)
                        {
                            if (f.Type is INamedTypeSymbol symbol)
                            {
                                Check(context, defaultExpression, symbol);
                            }
                        }
                        break;
                    }
                case IPropertyReferenceOperation propRef:
                    {
                        if (propRef.Property.Type is INamedTypeSymbol symbol)
                        {
                            Check(context, defaultExpression, symbol);
                        }

                        break;
                    }
                case ILocalReferenceOperation localRef:
                    {
                        if (localRef.Local.Type is INamedTypeSymbol symbol)
                        {
                            Check(context, defaultExpression, symbol);
                        }
                        break;
                    }
            }
        }

        private void Check(OperationAnalysisContext context, DefaultExpressionSyntax defaultExpression, INamedTypeSymbol symbol)
        {
            if (context.CancellationToken.IsCancellationRequested)
            {
                return;
            }

            if (symbol.OriginalDefinition.SpecialType != SpecialType.System_Nullable_T)
            {
                return;
            }

            if (!shouldReport(symbol))
            {
                return;
            }

            var model = context.Compilation.GetSemanticModel(defaultExpression.SyntaxTree);
            var typeInfo = model.GetTypeInfo(defaultExpression);

            if (Equals(typeInfo.Type, symbol.TypeArguments.FirstOrDefault()?.OriginalDefinition))
            {
                context.ReportDiagnostic(Diagnostic.Create(
                    SupportedDiagnostics[0],
                    defaultExpression.GetLocation(), SymbolDisplay.ToDisplayString(symbol,
                        SymbolDisplayFormat.CSharpShortErrorMessageFormat.WithParameterOptions(
                            SymbolDisplayParameterOptions.None))));
            }
        }

        protected NullableTypeAnalyzer(Func<INamedTypeSymbol, bool> shouldReport)
        {
            this.shouldReport = shouldReport ?? throw new ArgumentNullException(nameof(shouldReport));
        }
    }
}