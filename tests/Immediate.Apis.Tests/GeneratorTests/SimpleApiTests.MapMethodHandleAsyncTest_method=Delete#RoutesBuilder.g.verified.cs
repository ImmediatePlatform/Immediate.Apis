//HintName: RoutesBuilder.g.cs
using Microsoft.AspNetCore.Mvc;

#pragma warning disable CS1591

namespace Microsoft.AspNetCore.Builder;

public static partial class TestsRoutesBuilder
{
	public static IEndpointRouteBuilder MapTestsEndpoints(
		this IEndpointRouteBuilder app
	)
	{
		MapDummy_GetUsersQueryEndpoint(app);

		return app;
	}
}
