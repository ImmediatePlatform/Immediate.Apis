using Immediate.Apis.Analyzers;

namespace Immediate.Apis.Tests.AnalyzerTests;

public sealed class MissingHandlerAttributeAnalyzerTests
{
	[Test]
	[MethodDataSource(typeof(Utility), nameof(Utility.Methods))]
	public async Task MissingHandlerAttributeShouldError(string method) =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<MissingHandlerAttributeAnalyzer>(
			$$"""
			using System.Threading;
			using System.Threading.Tasks;
			using Immediate.Apis.Shared;
			using Immediate.Handlers.Shared;
			
			namespace Dummy;

			[Map{{method}}("/test")]
			public static class {|IAPI0001:GetUsersQuery|}
			{
				public record Query;

				private static async ValueTask<int> Handle(
					Query _,
					CancellationToken token)
				{
					return 0;
				}
			}
			"""
		).RunAsync();
}
