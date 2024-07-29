using System.Diagnostics.CodeAnalysis;

namespace Immediate.Apis.Shared;

/// <summary>
///		Applied to a class to indicate that minimal APIs registration should be generated
/// </summary>
/// <param name="route">
///		The route that the handler should be registered with
/// </param>
/// <param name="method">
///		
/// </param>
[AttributeUsage(AttributeTargets.Class)]
[SuppressMessage(
	"Performance",
	"CA1813:Avoid unsealed attributes",
	Justification = "Not used for runtime information"
)]
public class MapMethodAttribute(string route, string method) : Attribute
{
	/// <summary>
	///		The route that the handler should be registered with
	/// </summary>
	public string Route { get; } = route;

	/// <summary>
	///		The route that the handler should be registered with
	/// </summary>
	public string Method { get; } = method;
}
