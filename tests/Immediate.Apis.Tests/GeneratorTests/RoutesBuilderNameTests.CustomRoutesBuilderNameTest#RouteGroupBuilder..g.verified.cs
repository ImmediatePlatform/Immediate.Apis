//HintName: RouteGroupBuilder..g.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System.Diagnostics.CodeAnalysis;

#pragma warning disable CS1591

namespace Microsoft.AspNetCore.Builder;

public static partial class CustomRoutesBuilder
{
	public static RouteGroupBuilder MapCustomEndpoints(
		this IEndpointRouteBuilder app,
		[StringSyntax("Route")] string prefix = ""
	)
	{
		var group = app.MapGroup(prefix);

		MapDummy_GetUsersQueryEndpoint(group);

		return group;
	}
}
