using System.Diagnostics.CodeAnalysis;
using Immediate.Apis.Analyzers;

namespace Immediate.Apis.Tests.AnalyzerTests;

// NB: CS0234 is due to lack of `Microsoft.AspNetCore.Http.Abstractions` library
// TODO: figure out how to reference `Microsoft.AspNetCore.App.Ref`

[SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores", Justification = "Test names")]
public sealed class MissingTransformResultMethodAnalyzerTests
{
	[Test]
	[MethodDataSource(typeof(Utility), nameof(Utility.Methods))]
	public async Task ValidDefinitionShouldRaiseHiddenDiagnostic(string method)
	{
		await AnalyzerTestHelpers.CreateAnalyzerTest<MissingTransformResultMethodAnalyzer>(
			$$"""
			using System.Threading;
			using System.Threading.Tasks;
			using Immediate.Apis.Shared;
			using Immediate.Handlers.Shared;
			using Microsoft.AspNetCore.Authorization;
					
			namespace Dummy;

			[Handler]
			[Map{{method}}("/test")]
			public static class {|IAPI0007:GetUsersQuery|}
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

	[Test]
	[MethodDataSource(typeof(Utility), nameof(Utility.Methods))]
	public async Task ValidDefinition_WithExistingTransformResultMethod_ShouldNotRaiseHiddenDiagnostic(string method)
	{
		await AnalyzerTestHelpers.CreateAnalyzerTest<MissingTransformResultMethodAnalyzer>(
			$$"""
			using System.Threading;
			using System.Threading.Tasks;
			using Immediate.Apis.Shared;
			using Immediate.Handlers.Shared;
			using Microsoft.AspNetCore.Http.HttpResults;
			using Microsoft.AspNetCore.Http;
					
			namespace Dummy;

			[Handler]
			[Map{{method}}("/test")]
			public static class GetUsersQuery
			{
				internal static Results<Ok<int>, NotFound> TransformResult(int result)
				{
					return TypedResults.Ok(result);
				}

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

	[Test]
	[MethodDataSource(typeof(Utility), nameof(Utility.Methods))]
	public async Task InvalidDefinition_ShouldNotRaiseHiddenDiagnostic(string method)
	{
		await AnalyzerTestHelpers.CreateAnalyzerTest<MissingTransformResultMethodAnalyzer>(
			$$"""
			using System.Threading;
			using System.Threading.Tasks;
			using Immediate.Apis.Shared;
			using Immediate.Handlers.Shared;
			using Microsoft.AspNetCore.Http.HttpResults;
			using Microsoft.AspNetCore.Http;
					
			namespace Dummy;

			[Handler]
			[Map{{method}}("/test")]
			public static class GetUsersQuery
			{
				public record Query;
			}
			"""
		).RunAsync();
	}
}
