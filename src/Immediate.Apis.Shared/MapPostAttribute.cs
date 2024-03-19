using System.Diagnostics.CodeAnalysis;

namespace Immediate.Apis.Shared;

/// <inheritdoc />
public sealed class MapPostAttribute(
	[StringSyntax("Route")] string route
) : MapMethodAttribute(route);
