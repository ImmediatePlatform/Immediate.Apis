using System.Diagnostics.CodeAnalysis;

namespace Immediate.Apis.Shared;

/// <inheritdoc />
public sealed class MapDeleteAttribute(
	[StringSyntax("Route")] params string[] routes
) : MapMethodAttribute("DELETE", routes)
{
	/// <summary>
	///		Applied to a class to indicate that minimal APIs registration for a <c>DELETE</c> endpoint should be generated
	/// </summary>
	/// <param name="route">
	///		The route that the handler should be registered with
	/// </param>
	[SuppressMessage("Design", "CA1019:Define accessors for attribute arguments")]
	public MapDeleteAttribute(string route) : this([route]) { }
}
