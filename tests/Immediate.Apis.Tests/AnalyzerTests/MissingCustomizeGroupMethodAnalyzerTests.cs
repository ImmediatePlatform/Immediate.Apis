using System.Diagnostics.CodeAnalysis;
using Immediate.Apis.Analyzers;

namespace Immediate.Apis.Tests.AnalyzerTests;

[SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores", Justification = "Test names")]
public sealed class MissingCustomizeGroupMethodAnalyzerTests
{
	[Fact]
	public async Task ValidDefinitionShouldRaiseHiddenDiagnostic()
	{
		await AnalyzerTestHelpers.CreateAnalyzerTest<MissingCustomizeGroupMethodAnalyzer>(
			"""
			using System.Threading;
			using System.Threading.Tasks;
			using Immediate.Apis.Shared;
			using Immediate.Handlers.Shared;
			using Microsoft.AspNetCore.Authorization;
			using Microsoft.AspNetCore.Routing;
			
			namespace Dummy;
			
			[RouteGroup("/api/root")]
			public sealed class {|IAPI0012:Root|}
			{
			}
			"""
		).RunAsync(TestContext.Current.CancellationToken);
	}

	[Fact]
	public async Task ValidDefinition_WithExistingCustomizeGroupMethod_ShouldNotRaiseHiddenDiagnostic()
	{
		await AnalyzerTestHelpers.CreateAnalyzerTest<MissingCustomizeGroupMethodAnalyzer>(
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
				private static void CustomizeGroup(RouteGroupBuilder group) { }
			}
			"""
		).RunAsync(TestContext.Current.CancellationToken);
	}
}
