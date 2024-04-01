using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Immediate.Apis.FunctionalTests.Features.WeatherForecast;

[Handler]
[MapPost("/forecast")]
[AllowAnonymous]
public static partial class Post
{
	internal static void CustomizeEndpoint(IEndpointConventionBuilder endpoint)
		=> endpoint
			.WithDescription("Gets the current weather forecast");

	internal static Results<Ok<IReadOnlyList<Result>>, NotFound> TransformResult(IReadOnlyList<Result> result)
	{
		return TypedResults.NotFound();
	}

	public sealed record Query
	{
		public DateOnly Date { get; init; }
	}

	public sealed record Result
	{
		public required DateOnly Date { get; init; }
		public required int TemperatureC { get; init; }
		public required string? Summary { get; init; }
		public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
	}

	private static async ValueTask<IReadOnlyList<Result>> Handle(
		Query _,
		CancellationToken token
	)
	{
		await Task.Delay(1, token);
		return [
			new() { Date = new(2024, 1, 1), TemperatureC = 0, Summary = "Sunny and Freezing" },
		];
	}
}
