using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Immediate.Apis.FunctionalTests.Swagger.Features.WeatherForecast;

/// <summary>Find the current weather forecast.</summary>
[Handler]
[MapGet("/forecast/{id:int}", "/forecast/{id:int}/secondary")]
[AllowAnonymous]
public static partial class Get
{
	internal static void CustomizeEndpoint(IEndpointConventionBuilder endpoint)
		=> endpoint
			.WithDescription("Gets the current weather forecast");

	public sealed record Query
	{
		/// <summary>The forecast identifier.</summary>
		[FromRoute(Name = "id")]
		public required int Id { get; init; }

		/// <summary>Filters the forecast by its summary.</summary>
		public required string Filter { get; init; }
	}

	public sealed record Result
	{
		public required DateOnly Date { get; init; }
		public required int TemperatureC { get; init; }
		public required string? Summary { get; init; }
		public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
	}

	/// <returns>The matching forecasts.</returns>
	/// <response code="404">No forecast matches the identifier.</response>
	[ProducesResponseType<IReadOnlyList<Result>>(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
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
