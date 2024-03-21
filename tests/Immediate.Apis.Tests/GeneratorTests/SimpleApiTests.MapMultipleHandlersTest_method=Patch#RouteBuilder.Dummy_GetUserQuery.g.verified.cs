//HintName: RouteBuilder.Dummy_GetUserQuery.g.cs
using Microsoft.AspNetCore.Mvc;

#pragma warning disable CS1591

namespace Microsoft.AspNetCore.Builder;

public static partial class TestsRoutesBuilder
{
	private static void MapDummy_GetUserQueryEndpoint(IEndpointRouteBuilder app)
	{
		var endpoint = app
			.MapPatch(
				"/test",
				async (
					[AsParameters] global::Dummy.GetUserQuery.Query parameters,
					[FromServices] global::Dummy.GetUserQuery.Handler handler,
					CancellationToken token
				) => await handler.HandleAsync(parameters, token)
			);
	}
}
