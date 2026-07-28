using Immediate.Apis.Analyzers;

namespace Immediate.Apis.Tests.AnalyzerTests;

public sealed class RouteGroupClassAnalyzerTests
{
	[Fact]
	public async Task NestedRouteGroupShouldError() =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<RouteGroupClassAnalyzer>(
			"""
			using System.Threading;
			using System.Threading.Tasks;
			using Immediate.Apis.Shared;
			using Immediate.Handlers.Shared;
			
			namespace Dummy;

			public sealed partial class Outer
			{
				[RouteGroup("inner")]
				public sealed partial class {|IAPI0014:Inner|};
			}
			"""
		).RunAsync(TestContext.Current.CancellationToken);

	[Fact]
	public async Task NonNestedRouteGroupShouldNotError() =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<RouteGroupClassAnalyzer>(
			"""
			using System.Threading;
			using System.Threading.Tasks;
			using Immediate.Apis.Shared;
			using Immediate.Handlers.Shared;
			
			namespace Dummy;

			[RouteGroup("outer")]
			public sealed partial class Outer
			{
				[RouteGroup("inner")]
				public sealed partial class Inner;
			}
			"""
		).RunAsync(TestContext.Current.CancellationToken);
}
