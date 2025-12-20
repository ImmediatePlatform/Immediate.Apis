using System.Diagnostics.CodeAnalysis;

namespace Immediate.Apis.Shared;

/// <inheritdoc />
public sealed class MapPutAttribute(
	[StringSyntax("Route")] params string[] routes
) : MapMethodAttribute("PUT", routes);
