using Immediate.Apis.FunctionalTests.Filters;
using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Microsoft.AspNetCore.Authorization;

namespace Immediate.Apis.FunctionalTests.Features.Admin;

[Handler]
[MapGet("/admin")]
[EndpointFilter<ApiKeyEndpointFilter>]
[AllowAnonymous]
public static partial class Get
{
	public sealed record Query;

	private static async ValueTask<string> Handle(
		Query _,
		CancellationToken token
	)
	{
		await Task.Delay(1, token);
		return "Hello, Admin!";
	}
}
