using Immediate.Apis.Analyzers;

namespace Immediate.Apis.Tests.AnalyzerTests;

public sealed class DuplicateEndpointAnalyzerTests
{
	[Fact]
	public async Task DuplicateEndpointsShouldTriggerOnAllLocations() =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<DuplicateEndpointAnalyzer>(
			"""
			using System.Threading;
			using System.Threading.Tasks;
			using Immediate.Apis.Shared;
			using Immediate.Handlers.Shared;
			
			namespace Dummy;
			
			[{|IAPI0009:MapGet("/test")|}]
			public static class GetUsersQuery
			{
				public record Query;
			
				private static async ValueTask<int> Handle(
					Query _,
					CancellationToken token)
				{
					return 0;
				}
			}
			
			[{|IAPI0009:MapGet("/test")|}]
			public static class GetUsersQuery2
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
		).RunAsync(TestContext.Current.CancellationToken);

	[Fact]
	public async Task DuplicateEndpointsShouldNotTriggerUnnecessarily() =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<DuplicateEndpointAnalyzer>(
			"""
			using System.Threading;
			using System.Threading.Tasks;
			using Immediate.Apis.Shared;
			using Immediate.Handlers.Shared;
			
			namespace Dummy;
			
			[MapGet("/test")]
			public static class GetUsersQuery
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
		).RunAsync(TestContext.Current.CancellationToken);
}
