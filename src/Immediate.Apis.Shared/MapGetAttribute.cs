using System.Diagnostics.CodeAnalysis;

namespace Immediate.Apis.Shared;

/// <inheritdoc />
public sealed class MapGetAttribute(
	[StringSyntax("Route")] string route
) : MapMethodAttribute(route, "GET");
