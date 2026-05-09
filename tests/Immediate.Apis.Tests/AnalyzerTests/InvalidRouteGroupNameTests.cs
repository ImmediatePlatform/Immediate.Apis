using Immediate.Apis.Analyzers;

namespace Immediate.Apis.Tests.AnalyzerTests;

public sealed class InvalidRouteGroupNameTests
{
	[Theory]
	[MemberData(nameof(Utility.ValidRouteGroupNames), MemberType = typeof(Utility))]
	public async Task ValidRouteGroupNamesShouldNotError(string groupName) =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<InvalidRouteGroupNameAnalyzer>(
			$$"""
			using System.Threading;
			using System.Threading.Tasks;
			using Immediate.Apis.Shared;
			using Immediate.Handlers.Shared;

			namespace Dummy;

			[Handler]
			[MapGet("/test")]
			[RouteGroup("{{groupName}}")]
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
	[MemberData(nameof(Utility.InvalidRouteGroupNames), MemberType = typeof(Utility))]
	public async Task InvalidRouteGroupNamesShouldError(string groupName) =>
	await AnalyzerTestHelpers.CreateAnalyzerTest<InvalidRouteGroupNameAnalyzer>(
		$$"""
			using System.Threading;
			using System.Threading.Tasks;
			using Immediate.Apis.Shared;
			using Immediate.Handlers.Shared;

			namespace Dummy;

			[Handler]
			[MapGet("/test")]
			[{|IAPI0009:RouteGroup("{{groupName}}")|}]
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
