using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using ReviewHelp.Analyzers.Tests.Infrastructure;
using ReviewHelp.Analyzers.Usage;
using Xunit;

namespace ReviewHelp.Analyzers.Tests.Usage
{
    public sealed class NullableTypeAssignedDefaultOfUnderlyingTypeTests
    {
        private readonly DiagnosticAnalyzer analyzer = new NullableTypeAssignedDefaultOfUnderlyingType();

        [Fact]
        public async void CanIdentifyQueriesWithinIterations()
        {			
            var diagnostics = await TestHelper.GetDiagnosticsAsync(analyzer,
                @"using System;

class TestClass { 
	
    DateTime? dt3 = default(DateTime);
    public DateTime? dt4 {get; set;} = default(DateTime);

	void TestMethod() 
	{
        DateTime? dt1 = DateTime.Now;
		DateTime? dt = default(DateTime);
		DateTime? dt2 = null;
		dt2 = default(DateTime);
        dt3 = default(DateTime);
        dt4 = default(DateTime);
        dt3 = DateTime.Now;
        dt4 = DateTime.Now;    
        DateTimeOffset? do1 = default(DateTimeOffset);
        int? i1 = default(int);
        decimal? d1 = default(decimal);
        decimal? d2 = default(decimal?);
	}
}");

            Assert.Collection(diagnostics.OrderBy(x => x.Location.GetLineSpan().StartLinePosition.Line),
                d =>
                {
                    Assert.Equal(DiagnosticSeverity.Warning, d.Severity);
                    Assert.Equal(18, d.Location.GetLineSpan().StartLinePosition.Line);
                },
                d =>
                {
                    Assert.Equal(DiagnosticSeverity.Warning, d.Severity);
                    Assert.Equal(19, d.Location.GetLineSpan().StartLinePosition.Line);
                });
        }
    }
}