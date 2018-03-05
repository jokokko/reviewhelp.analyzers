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
    }
}