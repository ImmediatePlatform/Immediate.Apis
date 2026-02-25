using System.Diagnostics.CodeAnalysis;

namespace Immediate.Apis.Shared;

/// <inheritdoc />
public sealed class MapGetAttribute(
	[StringSyntax("Route")] params string[] routes
) : MapMethodAttribute("GET", routes)
{
	/// <summary>
	///		Applied to a class to indicate that minimal APIs registration for a <c>GET</c> endpoint should be generated
	/// </summary>
	/// <param name="route">
	///		The route that the handler should be registered with
	/// </param>
	[SuppressMessage("Design", "CA1019:Define accessors for attribute arguments")]
	public MapGetAttribute(string route) : this([route]) { }
}
