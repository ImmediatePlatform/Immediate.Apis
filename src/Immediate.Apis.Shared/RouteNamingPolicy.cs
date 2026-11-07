namespace Immediate.Apis.Shared;

/// <summary>Controls how source directory names become route segments.</summary>
public enum RouteNamingPolicy
{
	/// <summary>Capitalizes each word.</summary>
	PascalCase,
	/// <summary>Uses a lowercase initial word followed by capitalized words.</summary>
	CamelCase,
	/// <summary>Uses lowercase words without separators.</summary>
	LowerCase,
	/// <summary>Separates lowercase words with hyphens.</summary>
	KebabCase,
	/// <summary>Separates lowercase words with underscores.</summary>
	SnakeCase,
}
