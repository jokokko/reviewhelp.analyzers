using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using ReviewHelp.Analyzers.Tests.Infrastructure;
using ReviewHelp.Analyzers.Usage;
using Xunit;

namespace ReviewHelp.Analyzers.Tests.Usage
{
	public sealed class CaughtExceptionDiscardedAnalyzerTests
	{
		private readonly DiagnosticAnalyzer analyzer = new CaughtExceptionDiscardedAnalyzer();

		[Fact]
		public async void CanIdentifyDiscardedExceptions()
		{			
			var diagnostics = await TestHelper.GetDiagnosticsAsync(analyzer,
				@"using System;

class TestClass { 
	
    DateTime? dt3 = default(DateTime);
    public DateTime? dt4 {get; set;} = default(DateTime);

	void TestMethod() 
	{
		try {

		} catch (Exception e) when (e.Message == ""msg"")
		{
			throw;
			throw new Exception(""new without inner"");
			throw new Exception(""new with inner"", e);
		}
		try {

		} catch
		{			
			throw;
			throw new Exception(""new with inner"");
		}
		try {

		} catch
		{			
		}
	}
}");

			Assert.Collection(diagnostics.OrderBy(x => x.Location.GetLineSpan().StartLinePosition.Line),
				d =>
				{
					Assert.Equal(DiagnosticSeverity.Warning, d.Severity);
					Assert.Equal(14, d.Location.GetLineSpan().StartLinePosition.Line);
				},
				d =>
				{
					Assert.Equal(DiagnosticSeverity.Warning, d.Severity);
					Assert.Equal(22, d.Location.GetLineSpan().StartLinePosition.Line);
				});
		}
	}
}