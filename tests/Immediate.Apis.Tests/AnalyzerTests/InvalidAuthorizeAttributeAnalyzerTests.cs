using Immediate.Apis.Analyzers;

namespace Immediate.Apis.Tests.AnalyzerTests;

public sealed class InvalidAuthorizeAttributeAnalyzerTests
{
	[Fact]
	public async Task AuthorizeAloneShouldNotError() =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<InvalidAuthorizeAttributeAnalyzer>(
			$$"""
			using System.Threading;
			using System.Threading.Tasks;
			using Immediate.Apis.Shared;
			using Immediate.Handlers.Shared;
			using Microsoft.AspNetCore.Authorization;
					
			namespace Dummy;

			[Handler]
			[Authorize(Roles = "")]
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
		).RunAsync();

	[Theory]
	[MemberData(nameof(Utility.Methods), MemberType = typeof(Utility))]
	public async Task AuthorizeRolesShouldError(string method) =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<InvalidAuthorizeAttributeAnalyzer>(
			$$"""
			using System.Threading;
			using System.Threading.Tasks;
			using Immediate.Apis.Shared;
			using Immediate.Handlers.Shared;
			using Microsoft.AspNetCore.Authorization;
				
			namespace Dummy;

			[Handler]
			[Map{{method}}("/test")]
			[{|IAPI0002:Authorize(Roles = "")|}]
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
		).RunAsync();

	[Theory]
	[MemberData(nameof(Utility.Methods), MemberType = typeof(Utility))]
	public async Task AuthorizeAuthenticationSchemesShouldError(string method) =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<InvalidAuthorizeAttributeAnalyzer>(
			$$"""
			using System.Threading;
			using System.Threading.Tasks;
			using Immediate.Apis.Shared;
			using Immediate.Handlers.Shared;
			using Microsoft.AspNetCore.Authorization;
				
			namespace Dummy;

			[Handler]
			[Map{{method}}("/test")]
			[{|IAPI0002:Authorize(AuthenticationSchemes = "")|}]
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
		).RunAsync();

	[Theory]
	[MemberData(nameof(Utility.Methods), MemberType = typeof(Utility))]
	public async Task AuthorizeShouldNotError(string method) =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<InvalidAuthorizeAttributeAnalyzer>(
			$$"""
			using System.Threading;
			using System.Threading.Tasks;
			using Immediate.Apis.Shared;
			using Immediate.Handlers.Shared;
			using Microsoft.AspNetCore.Authorization;
				
			namespace Dummy;

			[Handler]
			[Map{{method}}("/test")]
			[Authorize]
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
		).RunAsync();

	[Theory]
	[MemberData(nameof(Utility.Methods), MemberType = typeof(Utility))]
	public async Task AuthorizeConstructorShouldNotError(string method) =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<InvalidAuthorizeAttributeAnalyzer>(
			$$"""
			using System.Threading;
			using System.Threading.Tasks;
			using Immediate.Apis.Shared;
			using Immediate.Handlers.Shared;
			using Microsoft.AspNetCore.Authorization;
				
			namespace Dummy;

			[Handler]
			[Map{{method}}("/test")]
			[Authorize("")]
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
		).RunAsync();

	[Theory]
	[MemberData(nameof(Utility.Methods), MemberType = typeof(Utility))]
	public async Task AuthorizePolicyShouldNotError(string method) =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<InvalidAuthorizeAttributeAnalyzer>(
			$$"""
			using System.Threading;
			using System.Threading.Tasks;
			using Immediate.Apis.Shared;
			using Immediate.Handlers.Shared;
			using Microsoft.AspNetCore.Authorization;
				
			namespace Dummy;

			[Handler]
			[Map{{method}}("/test")]
			[Authorize(Policy = "")]
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
		).RunAsync();

	[Theory]
	[MemberData(nameof(Utility.Methods), MemberType = typeof(Utility))]
	public async Task AuthorizeAndAllowAnonymousShouldWarn(string method) =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<InvalidAuthorizeAttributeAnalyzer>(
			$$"""
			using System.Threading;
			using System.Threading.Tasks;
			using Immediate.Apis.Shared;
			using Immediate.Handlers.Shared;
			using Microsoft.AspNetCore.Authorization;
				
			namespace Dummy;

			[Handler]
			[Map{{method}}("/test")]
			[Authorize]
			[AllowAnonymous]
			public static class {|IAPI0003:GetUsersQuery|}
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
