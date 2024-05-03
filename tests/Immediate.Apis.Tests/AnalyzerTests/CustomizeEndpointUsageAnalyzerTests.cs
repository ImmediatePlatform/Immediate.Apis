using Immediate.Apis.Analyzers;

namespace Immediate.Apis.Tests.AnalyzerTests;

public sealed class CustomizeEndpointUsageAnalyzerTests
{
	[Theory]
	[MemberData(nameof(Utility.Methods), MemberType = typeof(Utility))]
	public async Task ValidDefinitionShouldNotWarn(string method) =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<CustomizeEndpointUsageAnalyzer>(
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
			[Authorize(Roles = "")]
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

	[Theory]
	[MemberData(nameof(Utility.Methods), MemberType = typeof(Utility))]
	public async Task MultipleDefinitionShouldNotWarn(string method) =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<CustomizeEndpointUsageAnalyzer>(
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
			[Authorize(Roles = "")]
			public static class GetUsersQuery
			{
				internal static void CustomizeEndpoint(Microsoft.AspNetCore.Builder.IEndpointConventionBuilder endpoint)
					=> endpoint
						.WithDescription("");
			
				internal static void CustomizeEndpoint(int id)
					=> id.ToString();
			
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
	public async Task InvalidAccessibilityShouldWarn(string method) =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<CustomizeEndpointUsageAnalyzer>(
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
			[Authorize(Roles = "")]
			public static class GetUsersQuery
			{
				private static void {|IAPI0004:CustomizeEndpoint|}(Microsoft.AspNetCore.Builder.IEndpointConventionBuilder endpoint)
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
	public async Task InstanceMethodShouldWarn(string method) =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<CustomizeEndpointUsageAnalyzer>(
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
			[Authorize(Roles = "")]
			public class GetUsersQuery
			{
				internal void {|IAPI0004:CustomizeEndpoint|}(Microsoft.AspNetCore.Builder.IEndpointConventionBuilder endpoint)
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
	public async Task InvalidReturnShouldWarn(string method) =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<CustomizeEndpointUsageAnalyzer>(
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
			[Authorize(Roles = "")]
			public static class GetUsersQuery
			{
				internal static Microsoft.AspNetCore.Builder.IEndpointConventionBuilder {|IAPI0004:CustomizeEndpoint|}(Microsoft.AspNetCore.Builder.IEndpointConventionBuilder endpoint)
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
	public async Task InvalidParameterTypeShouldWarn(string method) =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<CustomizeEndpointUsageAnalyzer>(
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
