using System.Diagnostics.CodeAnalysis;
using Immediate.Apis.Analyzers;
using Immediate.Apis.CodeFixes;

namespace Immediate.Apis.Tests.CodeFixTests;

[SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores", Justification = "Test names")]
public sealed class MissingCustomizeGroupMethodCodeFixTests
{
	[Fact]
	public async Task ValidDefinition_ShouldAddCustomizeGroupMethod()
	{
		await CodeFixTestHelper
			.CreateCodeFixTest<MissingCustomizeGroupMethodAnalyzer, MissingCustomizeGroupMethodCodeFixProvider>(
				"""
				using System.Threading;
				using System.Threading.Tasks;
				using Immediate.Apis.Shared;
				using Immediate.Handlers.Shared;
				using Microsoft.AspNetCore.Authorization;
			
				namespace Dummy;
			
				[RouteGroup("/api/root")]
				public sealed class {|IAPI0012:Root|}
				{
				}
				""",
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
					private static void CustomizeGroup(RouteGroupBuilder group)
					{
					}
				}
				"""
			).RunAsync(TestContext.Current.CancellationToken);
	}

	[Fact]
	public async Task ValidDefinitionBraceless_ShouldAddCustomizeGroupMethod()
	{
		await CodeFixTestHelper
			.CreateCodeFixTest<MissingCustomizeGroupMethodAnalyzer, MissingCustomizeGroupMethodCodeFixProvider>(
				"""
				using System.Threading;
				using System.Threading.Tasks;
				using Immediate.Apis.Shared;
				using Immediate.Handlers.Shared;
				using Microsoft.AspNetCore.Authorization;
			
				namespace Dummy;
			
				[RouteGroup("/api/root")]
				public sealed class {|IAPI0012:Root|};
				""",
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
					private static void CustomizeGroup(RouteGroupBuilder group)
					{
					}
				}
				"""
			).RunAsync(TestContext.Current.CancellationToken);
	}
}
