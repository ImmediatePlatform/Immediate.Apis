using Microsoft.CodeAnalysis.CSharp;

namespace Immediate.Apis.Generators;

public sealed partial class ImmediateApisGenerator
{
	private sealed record AssemblyDefaults
	{
		public required string AssemblyName { get; init; }
		public required LanguageVersion LanguageVersion { get; init; }
	}

	private sealed record Method
	{
		public required string MapMethod { get; init; }
		public required string? HttpMethod { get; init; }
		public required EquatableReadOnlyList<string> Attributes { get; init; }
		public required string ParameterAttribute { get; init; }
		public required EquatableReadOnlyList<string> Routes { get; init; }

		public required string? Namespace { get; init; }
		public required Class Class { get; init; }
		public required string ClassFullName { get; init; }

		public required string ParameterType { get; init; }
		public required string ParameterTypeDoc { get; init; }

		public required bool AllowAnonymous { get; init; }
		public required bool Authorize { get; init; }
		public required string? AuthorizePolicy { get; init; }

		public required bool UseCustomization { get; init; }
		public required bool UseTransformMethod { get; init; }
		public required bool HasReturn { get; init; }

		public required string? Tags { get; init; }

		public required string? RouteGroupClassFullName { get; init; }
	}

	public sealed record Class
	{
		public required string Type { get; init; }
		public required string Name { get; init; }
	}

	public sealed record RouteGroupDefinition
	{
		public required string? Namespace { get; init; }
		public required EquatableReadOnlyList<Class> OuterClasses { get; init; }
		public required Class Class { get; init; }
		public required string? ClassFullName { get; init; }

		public required string? RouteGroupClassFullName { get; init; }

		public required string Route { get; init; }
		public required bool UseCustomization { get; init; }

		public required string? Tags { get; init; }
	}

	public sealed record RouteEndpoint
	{
		public required string Name { get; init; }
		public required string ClassFullName { get; init; }
		public required string? Tags { get; init; }
		public required EquatableReadOnlyList<string> Routes { get; init; }
	}

	public sealed record RouteGroup
	{
		public required RouteGroupDefinition Definition { get; init; }
		public required EquatableReadOnlyList<RouteTag> Tags { get; init; }
	}

	public sealed record RouteTag
	{
		public required string? Tag { get; init; }
		public required EquatableReadOnlyList<RouteEndpoint> Endpoints { get; init; }
		public required EquatableReadOnlyList<RouteGroup> Groups { get; init; }
	}
}
