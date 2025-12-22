//HintName: RouteBuilder.Dummy_GetUsersQuery.g.cs
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
		private static void MapDummy_GetUsersQueryEndpoint(IEndpointRouteBuilder app)
		{
			var endpoint = app
				.MapPatch(
					"/api/users",
					async (
						[FromBody] global::Dummy.GetUsersQuery.Query parameters,
						[FromServices] global::Dummy.GetUsersQuery.Handler handler,
						CancellationToken token
					) =>
					{
						var ret = await handler.HandleAsync(parameters, token);
						return ret;
					}
				);

			endpoint = app
				.MapPatch(
					"/v1/users",
					async (
						[FromBody] global::Dummy.GetUsersQuery.Query parameters,
						[FromServices] global::Dummy.GetUsersQuery.Handler handler,
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

	/// <remarks><see cref="global::Dummy.GetUsersQuery.Query" /> registered using <c>[FromBody]</c></remarks>
	partial class GetUsersQuery
	{
		public static IReadOnlyList<string> Routes { get; } =
		[
			"/api/users",
			"/v1/users",
		];
	}
}
