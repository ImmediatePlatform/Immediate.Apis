namespace Immediate.Apis.Tests.GeneratorTests;

public sealed class CustomizeEndpointsTests
{
	[Theory]
	[MemberData(nameof(Utility.Methods), MemberType = typeof(Utility))]
	public async Task MapMethodCustomizeEndpointTest(string method)
	{
		var result = GeneratorTestHelper.RunGenerator(
			$$"""
			using System.Threading;
			using System.Threading.Tasks;
			using Immediate.Apis.Shared;
			using Immediate.Handlers.Shared;
			using Microsoft.AspNetCore.Authorization;
			using Microsoft.AspNetCore.Http;
			
			namespace Dummy;

			[Handler]
			[Map{{method}}("/test")]
			public static partial class GetUsersQuery
			{
				internal static void CustomizeEndpoint(Microsoft.AspNetCore.Builder.IEndpointConventionBuilder endpoint)
					=> endpoint
						.WithDescription("");

				public record Query;

				private static ValueTask<int> HandleAsync(
					Query _,
					CancellationToken token)
				{
					return ValueTask.FromResult(0);
				}
			}
			""");

		Assert.Equal(
			[
				@"Immediate.Apis.Generators/Immediate.Apis.Generators.ImmediateApisGenerator/RouteBuilder.Dummy_GetUsersQuery.g.cs",
				@"Immediate.Apis.Generators/Immediate.Apis.Generators.ImmediateApisGenerator/RoutesBuilder.g.cs",
				@"Immediate.Handlers.Generators/Immediate.Handlers.Generators.ImmediateHandlers.ImmediateHandlersGenerator/IH.Dummy.GetUsersQuery.g.cs",
				@"Immediate.Handlers.Generators/Immediate.Handlers.Generators.ImmediateHandlers.ImmediateHandlersGenerator/IH.ServiceCollectionExtensions.g.cs",
			],
			result.GeneratedTrees.Select(t => t.FilePath.Replace('\\', '/'))
		);

		_ = await Verify(result).UseParameters(method);
	}
}
