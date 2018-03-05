using System;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace ReviewHelp.Analyzers.Usage
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class NullableTypeAssignedDefaultOfUnderlyingType : NullableTypeAnalyzer
    {
        private static readonly string[] Types = {"System.DateTime?", "System.DateTimeOffset?"};

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
            ImmutableArray.Create(Descriptors.RH1000NNullableTypeAssignedDefaultOfUnderlyingType);

        public NullableTypeAssignedDefaultOfUnderlyingType() : base(symbol =>
            !Types.Any(x => x.Equals(symbol.ToDisplayString(), StringComparison.OrdinalIgnoreCase)))
        {
        }
    }
}