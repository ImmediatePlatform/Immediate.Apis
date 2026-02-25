using System.Diagnostics.CodeAnalysis;

namespace Immediate.Apis.Shared;

/// <inheritdoc />
public sealed class MapPostAttribute(
	[StringSyntax("Route")] params string[] routes
) : MapMethodAttribute("POST", routes)
{
	/// <summary>
	///		Applied to a class to indicate that minimal APIs registration for a <c>POST</c> endpoint should be generated
	/// </summary>
	/// <param name="route">
	///		The route that the handler should be registered with
	/// </param>
	[SuppressMessage("Design", "CA1019:Define accessors for attribute arguments")]
	public MapPostAttribute(string route) : this([route]) { }
}
