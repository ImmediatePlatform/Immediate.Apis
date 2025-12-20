using System.Diagnostics.CodeAnalysis;

namespace Immediate.Apis.Shared;

/// <inheritdoc />
public sealed class MapPatchAttribute(
	[StringSyntax("Route")] params string[] routes
) : MapMethodAttribute("PATCH", routes);
