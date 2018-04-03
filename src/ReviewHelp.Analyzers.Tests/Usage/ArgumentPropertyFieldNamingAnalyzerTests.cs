using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using ReviewHelp.Analyzers.Tests.Infrastructure;
using ReviewHelp.Analyzers.Usage;
using Xunit;

namespace ReviewHelp.Analyzers.Tests.Usage
{
    public sealed class ArgumentPropertyFieldNamingAnalyzerTests
    {
        private readonly DiagnosticAnalyzer analyzer = new ArgumentPropertyFieldNamingAnalyzer();

        [Fact]
        public async void CanIdentifyNameDifference()
        {			
            var diagnostics = await TestHelper.GetDiagnosticsAsync(analyzer,
				@"using System;

class TestClass { 
	    
    public string StringProp { get; private set; }
	public string StringProp2 { get; private set; }
	public int IntField;

	public TestClass(string stringArg, string stringProp2, int intArg)
	{		
        StringProp = stringArg;
		IntField = intArg;
		StringProp2 = stringProp2;
	}
}");

            Assert.Collection(diagnostics.OrderBy(x => x.Location.GetLineSpan().StartLinePosition.Line),
                d =>
                {
                    Assert.Equal(DiagnosticSeverity.Warning, d.Severity);
                    Assert.Equal(10, d.Location.GetLineSpan().StartLinePosition.Line);
                },
				d =>
	            {
		            Assert.Equal(DiagnosticSeverity.Warning, d.Severity);
		            Assert.Equal(11, d.Location.GetLineSpan().StartLinePosition.Line);
	            });
        }
    }
}