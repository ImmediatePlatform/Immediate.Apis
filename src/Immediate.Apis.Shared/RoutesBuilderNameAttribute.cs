namespace Immediate.Apis.Shared;

/// <summary>
///		Applied to an assembly to override the name used for the generated routes builder class
///		(<c>{Name}RoutesBuilder</c>) and its <c>Map{Name}Endpoints</c> registration methods.
/// </summary>
/// <remarks>
///		When not specified, the name defaults to the assembly name with <c>.</c> and spaces removed.
///		The value must be a valid C# identifier.
/// </remarks>
[AttributeUsage(AttributeTargets.Assembly)]
public sealed class RoutesBuilderNameAttribute(string name) : Attribute
{
	/// <summary>
	///		The name to use in place of the assembly name when generating the routes builder class
	///		and its registration methods.
	/// </summary>
	public string Name { get; } = name;
}
