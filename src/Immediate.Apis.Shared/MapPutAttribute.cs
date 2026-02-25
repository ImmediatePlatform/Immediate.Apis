using System.Diagnostics.CodeAnalysis;

namespace Immediate.Apis.Shared;

/// <inheritdoc />
public sealed class MapPutAttribute(
	[StringSyntax("Route")] params string[] routes
) : MapMethodAttribute("PUT", routes)
{
	/// <summary>
	///		Applied to a class to indicate that minimal APIs registration for a <c>PUT</c> endpoint should be generated
	/// </summary>
	/// <param name="route">
	///		The route that the handler should be registered with
	/// </param>
	[SuppressMessage("Design", "CA1019:Define accessors for attribute arguments")]
	public MapPutAttribute(string route) : this([route]) { }
}

