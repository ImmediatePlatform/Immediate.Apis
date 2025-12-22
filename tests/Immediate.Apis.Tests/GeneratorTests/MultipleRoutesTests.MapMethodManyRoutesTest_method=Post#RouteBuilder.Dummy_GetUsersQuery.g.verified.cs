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
				.MapPost(
					"/route1",
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
				.MapPost(
					"/route2",
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
				.MapPost(
					"/route3",
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
				.MapPost(
					"/route4",
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
				.MapPost(
					"/route5",
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
				.MapPost(
					"/route6",
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
				.MapPost(
					"/route7",
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
				.MapPost(
					"/route8",
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
				.MapPost(
					"/route9",
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
				.MapPost(
					"/route10",
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
			"/route1",
			"/route2",
			"/route3",
			"/route4",
			"/route5",
			"/route6",
			"/route7",
			"/route8",
			"/route9",
			"/route10",
		];
	}
}
