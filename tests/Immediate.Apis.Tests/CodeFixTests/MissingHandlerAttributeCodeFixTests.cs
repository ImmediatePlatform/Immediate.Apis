using System.Diagnostics.CodeAnalysis;
using Immediate.Apis.Analyzers;
using Immediate.Apis.CodeFixes;

namespace Immediate.Apis.Tests.CodeFixTests;

[SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores", Justification = "Test names")]
public sealed class MissingHandlerAttributeCodeFixTests
{
	[Theory]
	[MemberData(nameof(Utility.Methods), MemberType = typeof(Utility))]
	public async Task MissingHandlerAttribute_ShouldAddAttributeAndUsing(string method)
	{
		await CodeFixTestHelper
			.CreateCodeFixTest<MissingHandlerAttributeAnalyzer, MissingHandlerAttributeCodeFixProvider>(
				$$"""
				using System.Threading;
				using System.Threading.Tasks;
				using Immediate.Apis.Shared;

				namespace Dummy;

				[Map{{method}}("/test")]
				public static class {|IAPI0001:GetUsersQuery|}
				{
					public record Query;
				
					private static async ValueTask<int> Handle(
						Query _,
						CancellationToken token)
					{
						return 0;
					}
				}
				""",
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
	public async Task MissingHandlerAttributeWithUsing_ShouldAddAttributeOnly(string method)
	{
		await CodeFixTestHelper
			.CreateCodeFixTest<MissingHandlerAttributeAnalyzer, MissingHandlerAttributeCodeFixProvider>(
				$$"""
				using System.Threading;
				using Immediate.Handlers.Shared;
				using System.Threading.Tasks;
				using Immediate.Apis.Shared;

				namespace Dummy;

				[Map{{method}}("/test")]
				public static class {|IAPI0001:GetUsersQuery|}
				{
					public record Query;
				
					private static async ValueTask<int> Handle(
						Query _,
						CancellationToken token)
					{
						return 0;
					}
				}
				""",
				$$"""
				using System.Threading;
				using Immediate.Handlers.Shared;
				using System.Threading.Tasks;
				using Immediate.Apis.Shared;

				namespace Dummy;

				[Handler]
				[Map{{method}}("/test")]
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
	}
}
