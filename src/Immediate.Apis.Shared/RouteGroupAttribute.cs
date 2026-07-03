using System.Diagnostics.CodeAnalysis;

namespace Immediate.Apis.Shared;

/// <summary>
///		Defines a group of endpoints, which may optionally be further customized.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public sealed class RouteGroupAttribute([StringSyntax("route")] string route) : Attribute
{
	/// <summary>
	///		The route the group will use as a prefix for all inherited endpoints.
	/// </summary>
	public string Route { get; } = route;
}
