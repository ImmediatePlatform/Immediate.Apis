using System.Diagnostics.CodeAnalysis;
using Immediate.Apis.Analyzers;
using Immediate.Apis.CodeFixes;

namespace Immediate.Apis.Tests.CodeFixTests;

[SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores", Justification = "Test names")]
public sealed class MissingTransformResultMethodCodeFixTests
{
	[Theory]
	[MemberData(nameof(Utility.Methods), MemberType = typeof(Utility))]
	public async Task ValidDefinition_ShouldAddTransformResultMethod(string method)
	{
		await CodeFixTestHelper
			.CreateCodeFixTest<MissingTransformResultMethodAnalyzer, MissingTransformResultMethodCodeFixProvider>(
				$$"""
				using System.Threading;
				using System.Threading.Tasks;
				using Immediate.Apis.Shared;
				using Immediate.Handlers.Shared;
				using System.Collections.Generic;

				namespace Dummy;

				[Handler]
				[Map{{method}}("/test")]
				public static class {|IAPI0007:GetUsersQuery|}
				{
					public record Query;
					public record Response;
				
					private static async ValueTask<IReadOnlyList<Response>> Handle(
						Query _,
						CancellationToken token)
					{
						return [];
					}
				}
				""",
				$$"""
				using System.Threading;
				using System.Threading.Tasks;
				using Immediate.Apis.Shared;
				using Immediate.Handlers.Shared;
				using System.Collections.Generic;

				namespace Dummy;

				[Handler]
				[Map{{method}}("/test")]
				public static class GetUsersQuery
				{
					internal static IReadOnlyList<Response> TransformResult(IReadOnlyList<Response> result)
					{
						return result;
					}

					public record Query;
					public record Response;
				
					private static async ValueTask<IReadOnlyList<Response>> Handle(
						Query _,
						CancellationToken token)
					{
						return [];
					}
				}
				"""
			).RunAsync();
	}
}
