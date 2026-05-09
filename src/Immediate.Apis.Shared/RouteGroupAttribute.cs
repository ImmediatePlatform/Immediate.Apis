namespace Immediate.Apis.Shared;

/// <summary>
///		Applied to a class to indicate that a minimal APIs route group registration should be generated
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public sealed class RouteGroupAttribute(string name) : Attribute
{

	/// <summary>
	///		The name of the route group. This will be used in the generated registration method name.
	/// </summary>
	public string Name { get; } = name;
}
