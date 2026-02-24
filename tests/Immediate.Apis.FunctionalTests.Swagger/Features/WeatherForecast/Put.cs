using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Immediate.Apis.FunctionalTests.Swagger.Features.WeatherForecast;

[Handler]
[MapPut("/forecast/{date:datetime}")]
[Authorize("Test")]
public static partial class Put
{
	public sealed record Command
	{
		/// <summary>
		/// The date of the temperature data
		/// </summary>
		[FromRoute]
		public required DateOnly Date { get; init; }

		/// <summary>
		/// The temperature for the specified date, in Celsius
		/// </summary>
		public required int TemperatureC { get; init; }

		/// <summary>
		/// Additional information about the date's weather
		/// </summary>
		public required string? Summary { get; init; }

		public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
	}

	/// <summary>Save Temperature Data</summary>
	/// <remarks>
	/// Saves temperature data for a particular date.
	/// </remarks>
	/// <response code="200">Data saved successfully</response>
	private static async ValueTask Handle(
		[AsParameters]
		Command _,
		CancellationToken token
	)
	{
		await Task.Delay(1, token);
	}
}
