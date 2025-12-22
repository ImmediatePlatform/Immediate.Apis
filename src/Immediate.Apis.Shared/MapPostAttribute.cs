using System.Diagnostics.CodeAnalysis;

namespace Immediate.Apis.Shared;

/// <inheritdoc />
public sealed class MapPostAttribute(
	[StringSyntax("Route")] params string[] routes
) : MapMethodAttribute("POST", routes)
{
	/// <inheritdoc />
	[Obsolete("Kept for binary compatibility. Do not use directly.")]
	[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
	[System.Runtime.CompilerServices.OverloadResolutionPriority(-1)]
	[SuppressMessage("Design", "CA1019:Define accessors for attribute arguments")]
	public MapPostAttribute(string route) : this([route]) { }
}
