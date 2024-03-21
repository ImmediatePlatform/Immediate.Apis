using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace Immediate.Apis.Generators;

public sealed partial class ImmediateApisGenerator
{
	private sealed record Method
	{
		public required string Route { get; init; }
		public required string ClassName { get; init; }
		public required string MethodName { get; init; }
		public required string ParameterType { get; init; }
		public required bool AllowAnonymous { get; init; }
		public required bool Authorize { get; init; }
		public required string? AuthorizePolicy { get; init; }
	}

	private static readonly string[] s_methodAttributes =
	[
		"Immediate.Apis.Shared.MapGetAttribute",
		"Immediate.Apis.Shared.MapPostAttribute",
		"Immediate.Apis.Shared.MapPutAttribute",
		"Immediate.Apis.Shared.MapPatchAttribute",
		"Immediate.Apis.Shared.MapDeleteAttribute",
	];
}
