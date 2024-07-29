﻿//HintName: RouteBuilder.Dummy_GetUserQuery.g.cs
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
				.MapGet(
					"/test",
					async (
						[AsParameters] global::Dummy.GetUserQuery.Query parameters,
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

	/// <remarks><see cref="global::Dummy.GetUserQuery.Query" /> registered using <c>[AsParameters]</c></remarks>
	partial class GetUserQuery;
}
