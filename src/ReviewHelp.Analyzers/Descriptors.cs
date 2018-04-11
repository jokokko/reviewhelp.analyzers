using Microsoft.CodeAnalysis;
using ReviewHelp.Analyzers.Infrastructure;

namespace ReviewHelp.Analyzers
{
    internal static class Descriptors
    {
        private static DiagnosticDescriptor Rule(string id, string title, RuleCategory category, DiagnosticSeverity defaultSeverity, string messageFormat, string description = null)
        {            
            return new DiagnosticDescriptor(id, title, messageFormat, category.Name, defaultSeverity, true, description, $"https://jokokko.github.io/reviewhelp.analyzers/rules/{id}");
        }

	    internal static readonly DiagnosticDescriptor RH1000NNullableTypeAssignedDefaultOfUnderlyingType = Rule("RH1000", "Nullable type assigned to default of underlying type", RuleCategory.Usage, DiagnosticSeverity.Warning, "Nullable type assigned to default of underlying type.");
        internal static readonly DiagnosticDescriptor RH1001NNullableDateTimeAssignedDefaultOfUnderlyingType = Rule("RH1001", "Nullable DateTime or DateTimeOffset assigned to default of underlying type", RuleCategory.Usage, DiagnosticSeverity.Warning, "Nullable DateTime or DateTimeOffset assigned to default of underlying type.");
	    internal static readonly DiagnosticDescriptor RH1002CaughtExceptionDiscarded = Rule("RH1002", "Caught exception discarded in throw", RuleCategory.Usage, DiagnosticSeverity.Warning, "Caught exception '{0}' discarded in throw.");
	    internal static readonly DiagnosticDescriptor RH1003CaughtExceptionNotCapturedDiscarded = Rule("RH1003", "Caught exception not captured and discarded in throw", RuleCategory.Usage, DiagnosticSeverity.Warning, "Caught exception not captured in a local variable and discarded in throw.");
        internal static readonly DiagnosticDescriptor RH1004ArgumentNameDiffersFromPropertyAssignedTo = Rule("RH1004", "Constructor argument name differs from property or field being assigned to", RuleCategory.Usage, DiagnosticSeverity.Warning, "Constructor argument name '{0}' differs from property or field '{1}' being assigned to.");
	    internal static readonly DiagnosticDescriptor RH1005CtorEnumerableArgCouldBeMaterialized = Rule("RH1005", "IEnumerable<T> assigned from constructor argument could be materialized", RuleCategory.Usage, DiagnosticSeverity.Info, "IEnumerable '{0}' assigned from constructor argument '{1}' could be materialized.");
	}
}