namespace Immediate.Apis.Tests.GeneratorTests;

public class ApiAllowAnonymousTests
{
	[Theory]
	[InlineData("Get")]
	[InlineData("Post")]
	[InlineData("Patch")]
	[InlineData("Put")]
	[InlineData("Delete")]
	public async Task MapMethodWithAllowAnonymousTest(string method)
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
			[AllowAnonymous]
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
