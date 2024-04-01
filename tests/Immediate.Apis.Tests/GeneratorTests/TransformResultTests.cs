namespace Immediate.Apis.Tests.GeneratorTests;

public sealed class TransformResultTests
{
	[Theory]
	[MemberData(nameof(Utility.Methods), MemberType = typeof(Utility))]
	public async Task TransformTest(string method)
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
				internal static double TransformResult(int x) => x;

				public record Query;

				private static ValueTask<int> Handle(
					Query _,
					CancellationToken token)
				{
					return 0;
				}
			}
			""");

		var result = driver.GetRunResult();

		Assert.Empty(result.Diagnostics);
		Assert.Equal(2, result.GeneratedTrees.Length);

		_ = await Verify(result)
			.UseParameters(method);
	}
}
