using Immediate.Apis.Analyzers;

namespace Immediate.Apis.Tests.AnalyzerTests;

public sealed class CustomizeGroupUsageAnalyzerTests
{
	[Fact]
	public async Task ValidDefinitionShouldNotWarn() =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<CustomizeGroupUsageAnalyzer>(
			"""
			using System.Threading;
			using System.Threading.Tasks;
			using Immediate.Apis.Shared;
			using Immediate.Handlers.Shared;
			using Microsoft.AspNetCore.Authorization;
			using Microsoft.AspNetCore.Routing;

			namespace Dummy;

			[RouteGroup("/api/root")]
			public sealed class Root
			{
				private static void CustomizeGroup(RouteGroupBuilder group) { }
			}
			"""
		).RunAsync(TestContext.Current.CancellationToken);

	[Fact]
	public async Task MultipleDefinitionShouldNotWarn() =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<CustomizeGroupUsageAnalyzer>(
			"""
			using System.Threading;
			using System.Threading.Tasks;
			using Immediate.Apis.Shared;
			using Immediate.Handlers.Shared;
			using Microsoft.AspNetCore.Authorization;
			using Microsoft.AspNetCore.Routing;

			namespace Dummy;

			[RouteGroup("/api/root")]
			public sealed class Root
			{
				private static void {|IAPI0011:CustomizeGroup|}(RouteGroupBuilder group) { }
				private static void {|IAPI0011:CustomizeGroup|}(int x) { }
			}
			"""
		).RunAsync(TestContext.Current.CancellationToken);

	[Fact]
	public async Task InvalidAccessibilityShouldWarn() =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<CustomizeGroupUsageAnalyzer>(
			"""
			using System.Threading;
			using System.Threading.Tasks;
			using Immediate.Apis.Shared;
			using Immediate.Handlers.Shared;
			using Microsoft.AspNetCore.Authorization;
			using Microsoft.AspNetCore.Routing;

			namespace Dummy;

			[RouteGroup("/api/root")]
			public sealed class Root
			{
				public static void {|IAPI0011:CustomizeGroup|}(RouteGroupBuilder group) { }
			}
			"""
		).RunAsync(TestContext.Current.CancellationToken);

	[Fact]
	public async Task InstanceMethodShouldWarn() =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<CustomizeGroupUsageAnalyzer>(
			"""
			using System.Threading;
			using System.Threading.Tasks;
			using Immediate.Apis.Shared;
			using Immediate.Handlers.Shared;
			using Microsoft.AspNetCore.Authorization;
			using Microsoft.AspNetCore.Routing;

			namespace Dummy;

			[RouteGroup("/api/root")]
			public sealed class Root
			{
				public void {|IAPI0011:CustomizeGroup|}(RouteGroupBuilder group) { }
			}
			"""
		).RunAsync(TestContext.Current.CancellationToken);

	[Fact]
	public async Task InvalidReturnShouldWarn() =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<CustomizeGroupUsageAnalyzer>(
			"""
			using System.Threading;
			using System.Threading.Tasks;
			using Immediate.Apis.Shared;
			using Immediate.Handlers.Shared;
			using Microsoft.AspNetCore.Authorization;
			using Microsoft.AspNetCore.Routing;

			namespace Dummy;

			[RouteGroup("/api/root")]
			public sealed class Root
			{
				public int {|IAPI0011:CustomizeGroup|}(RouteGroupBuilder group) { return 5; }
			}
			"""
		).RunAsync(TestContext.Current.CancellationToken);

	[Fact]
	public async Task InvalidParameterTypeShouldWarn() =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<CustomizeGroupUsageAnalyzer>(
			"""
			using System.Threading;
			using System.Threading.Tasks;
			using Immediate.Apis.Shared;
			using Immediate.Handlers.Shared;
			using Microsoft.AspNetCore.Authorization;
			using Microsoft.AspNetCore.Routing;

			namespace Dummy;

			[RouteGroup("/api/root")]
			public sealed class Root
			{
				public void {|IAPI0011:CustomizeGroup|}(int group) { }
			}
			"""
		).RunAsync(TestContext.Current.CancellationToken);
}
