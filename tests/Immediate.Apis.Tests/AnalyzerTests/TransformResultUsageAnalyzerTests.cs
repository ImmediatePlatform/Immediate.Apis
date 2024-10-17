using Immediate.Apis.Analyzers;

namespace Immediate.Apis.Tests.AnalyzerTests;

public sealed class TransformResultUsageAnalyzerTests
{
	[Test]
	[MethodDataSource(typeof(Utility), nameof(Utility.Methods))]
	public async Task ValidDefinitionShouldNotWarn(string method) =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<TransformResultUsageAnalyzer>(
			$$"""
			using System.Threading;
			using System.Threading.Tasks;
			using Immediate.Apis.Shared;
			using Immediate.Handlers.Shared;
					
			namespace Dummy;

			[Handler]
			[Map{{method}}("/test")]
			public static class GetUsersQuery
			{
				internal static double TransformResult(int value) => value;

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

	[Test]
	[MethodDataSource(typeof(Utility), nameof(Utility.Methods))]
	public async Task MultipleDefinitionShouldNotWarn(string method) =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<TransformResultUsageAnalyzer>(
			$$"""
			using System.Threading;
			using System.Threading.Tasks;
			using Immediate.Apis.Shared;
			using Immediate.Handlers.Shared;

			namespace Dummy;

			[Handler]
			[Map{{method}}("/test")]
			public static class GetUsersQuery
			{
				internal static double TransformResult(double value) => value;
				internal static double TransformResult(int value) => value;
			
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

	[Test]
	[MethodDataSource(typeof(Utility), nameof(Utility.Methods))]
	public async Task InvalidAccessibilityShouldWarn(string method) =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<TransformResultUsageAnalyzer>(
			$$"""
			using System.Threading;
			using System.Threading.Tasks;
			using Immediate.Apis.Shared;
			using Immediate.Handlers.Shared;
					
			namespace Dummy;

			[Handler]
			[Map{{method}}("/test")]
			public static class GetUsersQuery
			{
				private static double {|IAPI0005:TransformResult|}(int value) => value;
			
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

	[Test]
	[MethodDataSource(typeof(Utility), nameof(Utility.Methods))]
	public async Task InstanceMethodShouldWarn(string method) =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<TransformResultUsageAnalyzer>(
			$$"""
			using System.Threading;
			using System.Threading.Tasks;
			using Immediate.Apis.Shared;
			using Immediate.Handlers.Shared;
					
			namespace Dummy;

			[Handler]
			[Map{{method}}("/test")]
			public sealed class GetUsersQuery
			{
				internal double {|IAPI0005:TransformResult|}(int value) => value;
			
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

	[Test]
	[MethodDataSource(typeof(Utility), nameof(Utility.Methods))]
	public async Task InvalidParameterTypeShouldWarn(string method) =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<TransformResultUsageAnalyzer>(
			$$"""
			using System.Threading;
			using System.Threading.Tasks;
			using Immediate.Apis.Shared;
			using Immediate.Handlers.Shared;
					
			namespace Dummy;

			[Handler]
			[Map{{method}}("/test")]
			public static class GetUsersQuery
			{
				internal static double {|IAPI0005:TransformResult|}(double value) => value;
			
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

	[Test]
	[MethodDataSource(typeof(Utility), nameof(Utility.Methods))]
	public async Task VoidReturnShouldWarn(string method) =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<TransformResultUsageAnalyzer>(
			$$"""
			using System.Threading;
			using System.Threading.Tasks;
			using Immediate.Apis.Shared;
			using Immediate.Handlers.Shared;
					
			namespace Dummy;

			[Handler]
			[Map{{method}}("/test")]
			public static class GetUsersQuery
			{
				internal static void {|IAPI0005:TransformResult|}(int value) { }
			
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
