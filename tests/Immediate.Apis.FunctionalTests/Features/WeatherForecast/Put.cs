using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;

namespace Immediate.Apis.FunctionalTests.Features.WeatherForecast;

[Handler]
[MapPut("/forecast")]
public static partial class Put
{
	public sealed record Command
	{
		public required DateOnly Date { get; init; }
		public required int TemperatureC { get; init; }
		public required string? Summary { get; init; }
		public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
	}

	private static async ValueTask Handle(
		Command _,
		CancellationToken token
	)
	{
		await Task.Delay(1, token);
	}
}
