using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Microsoft.AspNetCore.Authorization;

namespace Immediate.Apis.FunctionalTests.Features.WeatherForecast;

[Handler]
[MapGet("/forecast")]
[AllowAnonymous]
public static partial class Get
{
	internal static void CustomizeEndpoint(IEndpointConventionBuilder endpoint)
	{
		_ = endpoint
			.WithDescription("Gets the current weather forecast");
	}

	private static async ValueTask<IReadOnlyList<Result>> Handle(
		Query _,
		CancellationToken token
	)
	{
		await Task.Delay(1, token);
		return
		[
			new Result { Date = new DateOnly(2024, 1, 1), TemperatureC = 0, Summary = "Sunny and Freezing" }
		];
	}

	public sealed record Query;

	public sealed record Result
	{
		public required DateOnly Date { get; init; }
		public required int TemperatureC { get; init; }
		public required string? Summary { get; init; }
		public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
	}
}
