namespace Immediate.Apis.Generators;

public sealed partial class ImmediateApisGenerator
{
	private sealed record Method
	{
		public required string HttpMethod { get; init; }
		public required string ParameterAttribute { get; init; }
		public required string Route { get; init; }

		public required string ClassName { get; init; }
		public required string ClassAsMethodName { get; init; }
		public required string ParameterType { get; init; }

		public required bool AllowAnonymous { get; init; }
		public required bool Authorize { get; init; }
		public required string? AuthorizePolicy { get; init; }

		public required bool UseCustomization { get; init; }
		public required bool UseTransformMethod { get; init; }
	}
}
