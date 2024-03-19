namespace Immediate.Apis.Shared;

/// <summary>
///		Applied to a class to indicate that minimal APIs registration should be generated
/// </summary>
/// <param name="route">
///		The route that the handler should be registered with
/// </param>
[AttributeUsage(AttributeTargets.Class)]
public abstract class MapMethodAttribute(string route) : Attribute
{
	/// <summary>
	///		The route that the handler should be registered with
	/// </summary>
	public string Route { get; } = route;
}
