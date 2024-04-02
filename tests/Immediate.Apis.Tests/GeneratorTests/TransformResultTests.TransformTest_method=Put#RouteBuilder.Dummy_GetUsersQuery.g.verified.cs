//HintName: RouteBuilder.Dummy_GetUsersQuery.g.cs
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

#pragma warning disable CS1591

namespace Microsoft.AspNetCore.Builder;

public static partial class TestsRoutesBuilder
{
	private static void MapDummy_GetUsersQueryEndpoint(IEndpointRouteBuilder app)
	{
		var endpoint = app
			.MapPut(
				"/test",
				async (
					[FromBody] global::Dummy.GetUsersQuery.Query parameters,
					[FromServices] global::Dummy.GetUsersQuery.Handler handler,
					CancellationToken token
				) =>
				{
					var ret = await handler.HandleAsync(parameters, token);
					return global::Dummy.GetUsersQuery.TransformResult(ret);
				}
			);
	}
}
