namespace Immediate.Apis.Generators;

public sealed partial class ImmediateApisGenerator
{
	private sealed record Method
	{
		public required string MapMethod { get; init; }
		public required string? HttpMethod { get; init; }
		public required string ParameterAttribute { get; init; }
		public required string Route { get; init; }

		public required string? Namespace { get; init; }
		public required Class Class { get; init; }
		public required string ClassFullName { get; init; }
		public required string ClassAsMethodName { get; init; }
		public required string ParameterType { get; init; }

		public required bool AllowAnonymous { get; init; }
		public required bool Authorize { get; init; }
		public required string? AuthorizePolicy { get; init; }

		public required bool UseCustomization { get; init; }
		public required bool UseTransformMethod { get; init; }
	}

	public sealed record Class
	{
		public required string Type { get; init; }
		public required string Name { get; init; }
	}
}
