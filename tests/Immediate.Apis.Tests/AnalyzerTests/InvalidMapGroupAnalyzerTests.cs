using Immediate.Apis.Analyzers;

namespace Immediate.Apis.Tests.AnalyzerTests;

public sealed class InvalidMapGroupAnalyzerTests
{
	[Theory]
	[MemberData(nameof(Utility.Methods), MemberType = typeof(Utility))]
	public async Task ValidMapGroupShouldNotError(string method) =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<InvalidMapGroupAnalyzer>(
			$$"""
			using System.Threading;
			using System.Threading.Tasks;
			using Immediate.Apis.Shared;
			using Immediate.Handlers.Shared;
			using Microsoft.AspNetCore.Authorization;

			namespace Dummy;

			[RouteGroup("api/root")]
			public sealed partial class Root;

			[Handler]
			[Map{{method}}("test")]
			[MapGroup<Root>]
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

	[Theory]
	[MemberData(nameof(Utility.Methods), MemberType = typeof(Utility))]
	public async Task InvalidMapGroupShouldError(string method) =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<InvalidMapGroupAnalyzer>(
			$$"""
			using System.Threading;
			using System.Threading.Tasks;
			using Immediate.Apis.Shared;
			using Immediate.Handlers.Shared;
			using Microsoft.AspNetCore.Authorization;

			namespace Dummy;

			public sealed partial class Root;

			[Handler]
			[Map{{method}}("test")]
			[{|IAPI0013:MapGroup<Root>|}]
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
