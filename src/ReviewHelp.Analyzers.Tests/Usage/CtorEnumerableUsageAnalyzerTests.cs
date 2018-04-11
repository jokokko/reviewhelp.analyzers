using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using ReviewHelp.Analyzers.Tests.Infrastructure;
using ReviewHelp.Analyzers.Usage;
using Xunit;

namespace ReviewHelp.Analyzers.Tests.Usage
{
    public sealed class CtorEnumerableUsageAnalyzerTests
    {
        private readonly DiagnosticAnalyzer analyzer = new CtorEnumerableUsageAnalyzer();

        [Fact]
        public async void CanIdentifyMaterializationPoints()
        {			
            var diagnostics = await TestHelper.GetDiagnosticsAsync(analyzer,
				@"using System;
using System.Collections.Generic;
using System.Linq;
class TestClass { 
	    
    public IEnumerable<int> Values { get; }
	public IEnumerable<int> Values1 { get; }
	public IEnumerable<int> Values2 { get; }
	public List<int> Values3 { get; }
	public IEnumerable<int> Values4 { get; }

	public TestClass(IEnumerable<int> values, IEnumerable<int> values2, IEnumerable<int> val3)
	{		
        Values = values;
		Values1 = values;
		Values2 = values2.ToArray();
		Values3 = val3.ToList();
		Values4 = Values;
	}
}");

            Assert.Collection(diagnostics.OrderBy(x => x.Location.GetLineSpan().StartLinePosition.Line),
                d =>
                {
                    Assert.Equal(DiagnosticSeverity.Info, d.Severity);
                    Assert.Equal(13, d.Location.GetLineSpan().StartLinePosition.Line);
                },
                d =>
                {
                    Assert.Equal(DiagnosticSeverity.Info, d.Severity);
                    Assert.Equal(14, d.Location.GetLineSpan().StartLinePosition.Line);
                });
        }
    }
}