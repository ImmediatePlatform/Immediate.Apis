using System.Diagnostics.CodeAnalysis;

namespace Immediate.Apis.Shared;

/// <inheritdoc />
public sealed class MapGetAttribute(
	[StringSyntax("Route")] params string[] routes
) : MapMethodAttribute("GET", routes);
