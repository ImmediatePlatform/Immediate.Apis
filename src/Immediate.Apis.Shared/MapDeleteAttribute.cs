using System.Diagnostics.CodeAnalysis;

namespace Immediate.Apis.Shared;

/// <inheritdoc />
public sealed class MapDeleteAttribute(
	[StringSyntax("Route")] string route
) : MapMethodAttribute(route, "DELETE");
