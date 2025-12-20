using System.Diagnostics.CodeAnalysis;

namespace Immediate.Apis.Shared;

/// <inheritdoc />
public sealed class MapPostAttribute(
	[StringSyntax("Route")] params string[] routes
) : MapMethodAttribute("POST", routes);
