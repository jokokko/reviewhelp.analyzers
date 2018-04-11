using Microsoft.CodeAnalysis;

namespace ReviewHelp.Analyzers.Infrastructure
{
	internal static class Formats
	{
		public static SymbolDisplayFormat FqfWithoutGenericType { get; } =
			new SymbolDisplayFormat(
				SymbolDisplayGlobalNamespaceStyle.Omitted,
				SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces,
				SymbolDisplayGenericsOptions.None,
				miscellaneousOptions:
				SymbolDisplayMiscellaneousOptions.EscapeKeywordIdentifiers | SymbolDisplayMiscellaneousOptions.ExpandNullable);
	}
}