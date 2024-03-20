namespace Immediate.Apis.Tests.GeneratorTests;

public sealed class ApiEndpointFilterTests
{
	public const string DummyEndpointFilterName = "DummyEndpointFilter";

	[Theory]
	[MemberData(nameof(Utility.Methods), MemberType = typeof(Utility))]
	public async Task MapMethodWithMultipleEndpointFiltersTest(string method)
	{
		var driver = GeneratorTestHelper.GetDriver(
			$$"""
			using System.Threading.Tasks;
			using Immediate.Apis.Shared;
			using Immediate.Handlers.Shared;
			using Microsoft.AspNetCore.Authorization;
			
			namespace Dummy;

			[Handler]
			[Map{{method}}("/test")]
			[EndpointFilter<{{DummyEndpointFilterName}}>]
			[EndpointFilter<{{DummyEndpointFilterName}}>]
			public static class GetUsersQuery
			{
				public record Query;

				private static ValueTask<int> HandleAsync(
					Query _,
					CancellationToken token)
				{
					return 0;
				}
			}
			""");

		var result = driver.GetRunResult();

		Assert.Empty(result.Diagnostics);
		_ = Assert.Single(result.GeneratedTrees);

		_ = await Verify(result)
			.UseParameters(method);
	}
}
