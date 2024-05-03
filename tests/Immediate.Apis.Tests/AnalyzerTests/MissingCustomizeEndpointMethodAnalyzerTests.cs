using System.Diagnostics.CodeAnalysis;
using Immediate.Apis.Analyzers;

namespace Immediate.Apis.Tests.AnalyzerTests;

// NB: CS0234 is due to lack of `Microsoft.AspNetCore.Http.Abstractions` library
// TODO: figure out how to reference `Microsoft.AspNetCore.App.Ref`

[SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores", Justification = "Test names")]
public sealed class MissingCustomizeEndpointMethodAnalyzerTests
{
	[Theory]
	[MemberData(nameof(Utility.Methods), MemberType = typeof(Utility))]
	public async Task ValidDefinitionShouldRaiseHiddenDiagnostic(string method)
	{
		await AnalyzerTestHelpers.CreateAnalyzerTest<MissingCustomizeEndpointMethodAnalyzer>(
			$$"""
			using System.Threading;
			using System.Threading.Tasks;
			using Immediate.Apis.Shared;
			using Immediate.Handlers.Shared;
			using Microsoft.AspNetCore.Authorization;
					
			namespace Dummy;

			[Handler]
			[Map{{method}}("/test")]
			public static class {|IAPI0006:GetUsersQuery|}
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

	[Theory]
	[MemberData(nameof(Utility.Methods), MemberType = typeof(Utility))]
	public async Task ValidDefinition_WithExistingCustomizeEndpointMethod_ShouldNotRaiseHiddenDiagnostic(string method)
	{
		await AnalyzerTestHelpers.CreateAnalyzerTest<MissingCustomizeEndpointMethodAnalyzer>(
			$$"""
			using System.Threading;
			using System.Threading.Tasks;
			using Immediate.Apis.Shared;
			using Immediate.Handlers.Shared;
			using Microsoft.AspNetCore.Authorization;
			using Microsoft.AspNetCore.Http;

			namespace Dummy;

			[Handler]
			[Map{{method}}("/test")]
			public static class GetUsersQuery
			{
				internal static void CustomizeEndpoint(Microsoft.AspNetCore.Builder.IEndpointConventionBuilder endpoint)
					=> endpoint
						.WithDescription("");

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
}
