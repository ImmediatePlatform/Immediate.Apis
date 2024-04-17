using System.Diagnostics.CodeAnalysis;
using Immediate.Apis.Analyzers;
using Immediate.Apis.CodeFixes;

namespace Immediate.Apis.Tests.CodeFixTests;

[SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores", Justification = "Test names")]
public sealed class MissingCustomizeEndpointMethodCodeFixTests
{
	[Theory]
	[MemberData(nameof(Utility.Methods), MemberType = typeof(Utility))]
	public async Task ValidDefinition_ShouldAddCustomizeEndpointMethod(string method)
	{
		await CodeFixTestHelper
			.CreateCodeFixTest<MissingCustomizeEndpointMethodAnalyzer, MissingCustomizeEndpointMethodCodeFixProvider>(
				$$"""
				using System.Threading;
				using System.Threading.Tasks;
				using Immediate.Apis.Shared;
				using Immediate.Handlers.Shared;

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
				""",
				$$"""
				using System.Threading;
				using System.Threading.Tasks;
				using Immediate.Apis.Shared;
				using Immediate.Handlers.Shared;
				using Microsoft.AspNetCore.{|CS0234:Builder|};

				namespace Dummy;

				[Handler]
				[Map{{method}}("/test")]
				public static class GetUsersQuery
				{
					internal static void CustomizeEndpoint({|CS0246:IEndpointConventionBuilder|} endpoint) => endpoint.WithDescription("");
						
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
