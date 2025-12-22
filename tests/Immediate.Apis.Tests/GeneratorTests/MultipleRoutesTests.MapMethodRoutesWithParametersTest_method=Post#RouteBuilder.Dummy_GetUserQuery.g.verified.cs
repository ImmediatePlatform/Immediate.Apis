//HintName: RouteBuilder.Dummy_GetUserQuery.g.cs
using System.Collections.Generic;
using System.Threading;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

#pragma warning disable CS1591

namespace Microsoft.AspNetCore.Builder
{
	public static partial class TestsRoutesBuilder
	{
		private static void MapDummy_GetUserQueryEndpoint(IEndpointRouteBuilder app)
		{
			var endpoint = app
				.MapPost(
					"/api/users/{id}",
					async (
						[FromBody] global::Dummy.GetUserQuery.Query parameters,
						[FromServices] global::Dummy.GetUserQuery.Handler handler,
						CancellationToken token
					) =>
					{
						var ret = await handler.HandleAsync(parameters, token);
						return ret;
					}
				);

			endpoint = app
				.MapPost(
					"/v1/users/{id}",
					async (
						[FromBody] global::Dummy.GetUserQuery.Query parameters,
						[FromServices] global::Dummy.GetUserQuery.Handler handler,
						CancellationToken token
					) =>
					{
						var ret = await handler.HandleAsync(parameters, token);
						return ret;
					}
				);

		}
	}
}

namespace Dummy
{

	/// <remarks><see cref="global::Dummy.GetUserQuery.Query" /> registered using <c>[FromBody]</c></remarks>
	partial class GetUserQuery
	{
		public static IReadOnlyList<string> Routes { get; } =
		[
			"/api/users/{id}",
			"/v1/users/{id}",
		];
	}
}
