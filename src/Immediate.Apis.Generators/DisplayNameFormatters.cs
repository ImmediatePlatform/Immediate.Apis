using Microsoft.CodeAnalysis;

namespace Immediate.Apis.Generators;

internal static class DisplayNameFormatters
{
	public static readonly SymbolDisplayFormat FullyQualifiedWithNullableFormat =
		SymbolDisplayFormat.FullyQualifiedFormat
			.WithMiscellaneousOptions(
				SymbolDisplayFormat.FullyQualifiedFormat.MiscellaneousOptions
				| SymbolDisplayMiscellaneousOptions.IncludeNullableReferenceTypeModifier
			);

}
