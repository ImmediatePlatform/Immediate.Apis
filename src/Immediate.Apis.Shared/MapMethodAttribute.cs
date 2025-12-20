using System.Diagnostics.CodeAnalysis;

namespace Immediate.Apis.Shared;

/// <summary>
///		Applied to a class to indicate that minimal APIs registration should be generated
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
[SuppressMessage(
	"Performance",
	"CA1813:Avoid unsealed attributes",
	Justification = "Not used for runtime information"
)]
public class MapMethodAttribute : Attribute
{
	/// <summary>
	///		Applied to a class to indicate that minimal APIs registration should be generated
	/// </summary>
	/// <param name="route">
	///		The route that the handler should be registered with
	/// </param>
	/// <param name="method">
	///		The HTTP method that the handler should be registered with
	/// </param>
	[SuppressMessage("Design", "CA1019:Define accessors for attribute arguments")]
	public MapMethodAttribute(string route, string method)
	{
		Routes = [route];
		Method = method;
	}

	/// <summary>
	///		Applied to a class to indicate that minimal APIs registration should be generated
	/// </summary>
	/// <param name="method">
	///		The HTTP method that the handler should be registered with
	/// </param>
	/// <param name="routes">
	///		The routes that the handler should be registered with
	/// </param>
	public MapMethodAttribute(string method, [StringSyntax("Route")] params string[] routes)
	{
		Routes = routes;
		Method = method;
	}

	/// <summary>
	///		The routes that the handler should be registered with
	/// </summary>
	public string[] Routes { get; }

	/// <summary>
	///		The HTTP method that the handler should be registered with
	/// </summary>
	public string Method { get; }
}
