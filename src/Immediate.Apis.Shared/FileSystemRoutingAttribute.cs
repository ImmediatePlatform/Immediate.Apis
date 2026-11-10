namespace Immediate.Apis.Shared;

/// <summary>Enables directory-based routes for handlers whose mapping attribute has no route.</summary>
[AttributeUsage(AttributeTargets.Assembly)]
public sealed class FileSystemRoutingAttribute : Attribute
{
	/// <summary>The project-relative directory containing the endpoints. Defaults to the project directory.</summary>
	public string RootDirectory { get; init; } = "";

	/// <summary>The naming policy applied to directory segments. Defaults to lowercase.</summary>
	public RouteNamingPolicy NamingPolicy { get; init; } = RouteNamingPolicy.LowerCase;
}
