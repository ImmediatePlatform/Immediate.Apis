using Immediate.Apis.Analyzers;

namespace Immediate.Apis.Tests.AnalyzerTests;

// NB: CS0234 is due to lack of `Microsoft.AspNetCore.Http.Abstractions` library
// TODO: figure out how to reference `Microsoft.AspNetCore.App.Ref`

public sealed class CustomizeEndpointUsageAnalyzerTests
{
	[Theory]
	[MemberData(nameof(Utility.Methods), MemberType = typeof(Utility))]
	public async Task ValidDefinitionShouldNotError(string method) =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<CustomizeEndpointUsageAnalyzer>(
			$$"""
			using System.Threading;
			using System.Threading.Tasks;
			using Immediate.Apis.Shared;
			using Immediate.Handlers.Shared;
			using Microsoft.AspNetCore.Authorization;
					
			namespace Dummy;

			[Handler]
			[Map{{method}}("/test")]
			[Authorize(Roles = "")]
			public static class GetUsersQuery
			{
				internal static void CustomizeEndpoint(Microsoft.AspNetCore.{|CS0234:Builder|}.IEndpointConventionBuilder endpoint)
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

	[Theory]
	[MemberData(nameof(Utility.Methods), MemberType = typeof(Utility))]
	public async Task InvalidAccessibilityShouldError(string method) =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<CustomizeEndpointUsageAnalyzer>(
			$$"""
			using System.Threading;
			using System.Threading.Tasks;
			using Immediate.Apis.Shared;
			using Immediate.Handlers.Shared;
			using Microsoft.AspNetCore.Authorization;
					
			namespace Dummy;

			[Handler]
			[Map{{method}}("/test")]
			[Authorize(Roles = "")]
			public static class GetUsersQuery
			{
				private static void {|IAPI0004:CustomizeEndpoint|}(Microsoft.AspNetCore.{|CS0234:Builder|}.IEndpointConventionBuilder endpoint)
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

	[Theory]
	[MemberData(nameof(Utility.Methods), MemberType = typeof(Utility))]
	public async Task InstanceMethodShouldError(string method) =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<CustomizeEndpointUsageAnalyzer>(
			$$"""
			using System.Threading;
			using System.Threading.Tasks;
			using Immediate.Apis.Shared;
			using Immediate.Handlers.Shared;
			using Microsoft.AspNetCore.Authorization;
					
			namespace Dummy;

			[Handler]
			[Map{{method}}("/test")]
			[Authorize(Roles = "")]
			public class GetUsersQuery
			{
				internal void {|IAPI0004:CustomizeEndpoint|}(Microsoft.AspNetCore.{|CS0234:Builder|}.IEndpointConventionBuilder endpoint)
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

	[Theory]
	[MemberData(nameof(Utility.Methods), MemberType = typeof(Utility))]
	public async Task InvalidReturnShouldError(string method) =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<CustomizeEndpointUsageAnalyzer>(
			$$"""
			using System.Threading;
			using System.Threading.Tasks;
			using Immediate.Apis.Shared;
			using Immediate.Handlers.Shared;
			using Microsoft.AspNetCore.Authorization;
					
			namespace Dummy;

			[Handler]
			[Map{{method}}("/test")]
			[Authorize(Roles = "")]
			public static class GetUsersQuery
			{
				internal static Microsoft.AspNetCore.{|CS0234:Builder|}.IEndpointConventionBuilder {|IAPI0004:CustomizeEndpoint|}(Microsoft.AspNetCore.{|CS0234:Builder|}.IEndpointConventionBuilder endpoint)
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

	[Theory]
	[MemberData(nameof(Utility.Methods), MemberType = typeof(Utility))]
	public async Task InvalidParameterTypeShouldError(string method) =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<CustomizeEndpointUsageAnalyzer>(
			$$"""
			using System.Threading;
			using System.Threading.Tasks;
			using Immediate.Apis.Shared;
			using Immediate.Handlers.Shared;
			using Microsoft.AspNetCore.Authorization;
					
			namespace Dummy;

			[Handler]
			[Map{{method}}("/test")]
			[Authorize(Roles = "")]
			public static class GetUsersQuery
			{
				internal static void {|IAPI0004:CustomizeEndpoint|}(int endpoint)
					=> endpoint.ToString();

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
