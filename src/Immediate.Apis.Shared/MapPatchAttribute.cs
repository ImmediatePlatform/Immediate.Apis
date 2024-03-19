using System.Diagnostics.CodeAnalysis;

namespace Immediate.Apis.Shared;

/// <inheritdoc />
public sealed class MapPatchAttribute(
	[StringSyntax("Route")] string route
) : MapMethodAttribute(route);
