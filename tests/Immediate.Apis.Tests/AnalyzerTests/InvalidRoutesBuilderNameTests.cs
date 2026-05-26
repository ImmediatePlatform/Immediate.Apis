using Immediate.Apis.Analyzers;

namespace Immediate.Apis.Tests.AnalyzerTests;

public sealed class InvalidRoutesBuilderNameTests
{
	[Theory]
	[MemberData(nameof(Utility.ValidRoutesBuilderNames), MemberType = typeof(Utility))]
	public async Task ValidRoutesBuilderNamesShouldNotError(string name) =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<InvalidRoutesBuilderNameAnalyzer>(
			$$"""
			using System.Threading;
			using System.Threading.Tasks;
			using Immediate.Apis.Shared;
			using Immediate.Handlers.Shared;

			[assembly: RoutesBuilderName("{{name}}")]

			namespace Dummy;

			[Handler]
			[MapGet("/test")]
			public static partial class GetUsersQuery
			{
				public record Query;

				private static ValueTask<int> Handle(
					Query _,
					CancellationToken token)
				{
					return ValueTask.FromResult(0);
				}
			}
			""").RunAsync(TestContext.Current.CancellationToken);

	[Theory]
	[MemberData(nameof(Utility.InvalidRoutesBuilderNames), MemberType = typeof(Utility))]
	public async Task InvalidRoutesBuilderNamesShouldError(string name) =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<InvalidRoutesBuilderNameAnalyzer>(
			$$"""
			using System.Threading;
			using System.Threading.Tasks;
			using Immediate.Apis.Shared;
			using Immediate.Handlers.Shared;

			[assembly: {|IAPI0011:RoutesBuilderName("{{name}}")|}]

			namespace Dummy;

			[Handler]
			[MapGet("/test")]
			public static partial class GetUsersQuery
			{
				public record Query;

				private static ValueTask<int> Handle(
					Query _,
					CancellationToken token)
				{
					return ValueTask.FromResult(0);
				}
			}
			""").RunAsync(TestContext.Current.CancellationToken);
}
