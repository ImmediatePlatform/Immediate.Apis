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
				.MapDelete(
					"/route1",
					async (
						[AsParameters] global::Dummy.GetUsersQuery.Query parameters,
						[FromServices] global::Dummy.GetUsersQuery.Handler handler,
						CancellationToken token
					) =>
					{
						var ret = await handler.HandleAsync(parameters, token);
						return ret;
					}
				);

			endpoint = app
				.MapDelete(
					"/route2",
					async (
						[AsParameters] global::Dummy.GetUsersQuery.Query parameters,
						[FromServices] global::Dummy.GetUsersQuery.Handler handler,
						CancellationToken token
					) =>
					{
						var ret = await handler.HandleAsync(parameters, token);
						return ret;
					}
				);

			endpoint = app
				.MapDelete(
					"/route3",
					async (
						[AsParameters] global::Dummy.GetUsersQuery.Query parameters,
						[FromServices] global::Dummy.GetUsersQuery.Handler handler,
						CancellationToken token
					) =>
					{
						var ret = await handler.HandleAsync(parameters, token);
						return ret;
					}
				);

			endpoint = app
				.MapDelete(
					"/route4",
					async (
						[AsParameters] global::Dummy.GetUsersQuery.Query parameters,
						[FromServices] global::Dummy.GetUsersQuery.Handler handler,
						CancellationToken token
					) =>
					{
						var ret = await handler.HandleAsync(parameters, token);
						return ret;
					}
				);

			endpoint = app
				.MapDelete(
					"/route5",
					async (
						[AsParameters] global::Dummy.GetUsersQuery.Query parameters,
						[FromServices] global::Dummy.GetUsersQuery.Handler handler,
						CancellationToken token
					) =>
					{
						var ret = await handler.HandleAsync(parameters, token);
						return ret;
					}
				);

			endpoint = app
				.MapDelete(
					"/route6",
					async (
						[AsParameters] global::Dummy.GetUsersQuery.Query parameters,
						[FromServices] global::Dummy.GetUsersQuery.Handler handler,
						CancellationToken token
					) =>
					{
						var ret = await handler.HandleAsync(parameters, token);
						return ret;
					}
				);

			endpoint = app
				.MapDelete(
					"/route7",
					async (
						[AsParameters] global::Dummy.GetUsersQuery.Query parameters,
						[FromServices] global::Dummy.GetUsersQuery.Handler handler,
						CancellationToken token
					) =>
					{
						var ret = await handler.HandleAsync(parameters, token);
						return ret;
					}
				);

			endpoint = app
				.MapDelete(
					"/route8",
					async (
						[AsParameters] global::Dummy.GetUsersQuery.Query parameters,
						[FromServices] global::Dummy.GetUsersQuery.Handler handler,
						CancellationToken token
					) =>
					{
						var ret = await handler.HandleAsync(parameters, token);
						return ret;
					}
				);

			endpoint = app
				.MapDelete(
					"/route9",
					async (
						[AsParameters] global::Dummy.GetUsersQuery.Query parameters,
						[FromServices] global::Dummy.GetUsersQuery.Handler handler,
						CancellationToken token
					) =>
					{
						var ret = await handler.HandleAsync(parameters, token);
						return ret;
					}
				);

			endpoint = app
				.MapDelete(
					"/route10",
					async (
						[AsParameters] global::Dummy.GetUsersQuery.Query parameters,
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

	/// <remarks><see cref="global::Dummy.GetUsersQuery.Query" /> registered using <c>[AsParameters]</c></remarks>
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
