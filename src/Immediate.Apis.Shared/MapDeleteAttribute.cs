using System.Diagnostics.CodeAnalysis;

namespace Immediate.Apis.Shared;

/// <inheritdoc />
public sealed class MapDeleteAttribute(
	[StringSyntax("Route")] params string[] routes
) : MapMethodAttribute("DELETE", routes);
