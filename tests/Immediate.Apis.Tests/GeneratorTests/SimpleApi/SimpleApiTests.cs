namespace Immediate.Apis.Tests.GeneratorTests.SimpleHandler;

public class SimpleApiTests
{
	[Theory]
	[InlineData("Get")]
	[InlineData("Post")]
	[InlineData("Patch")]
	[InlineData("Put")]
	[InlineData("Delete")]
	public async Task MapMethodTest(string method)
	{
		var driver = GeneratorTestHelper.GetDriver(
			$$"""
			using System.Threading.Tasks;
			using Immediate.Apis.Shared;
			using Immediate.Handlers.Shared;
			
			namespace Dummy;

			[Handler]
			[Map{{method}}("/test")]
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

		_ = await Verify(result)
			.UseParameters(method);
	}
}
