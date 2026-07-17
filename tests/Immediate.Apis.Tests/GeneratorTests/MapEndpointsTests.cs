namespace Immediate.Apis.Tests.GeneratorTests;

public sealed class MapEndpointsTests
{
	[Theory]
	[MemberData(nameof(Frameworks))]
	public async Task ValidAddServicesMethod(string framework)
	{
		var result = GeneratorTestHelper.RunGenerator(
			"""
			using Immediate.Apis.Shared;
			using Microsoft.Extensions.DependencyInjection;
			""",
			["Endpoints"]
		);

		Assert.Equal(
			[
				"Immediate.Apis.Generators/Immediate.Apis.Generators.ImmediateApisGenerator/IA.MapEndpoints.g.cs",
				"Immediate.Handlers.Generators/Immediate.Handlers.Generators.ImmediateHandlersGenerator/IH.ServiceCollectionExtensions.g.cs",
			],
			result.GeneratedTrees.Select(t => t.FilePath.Replace('\\', '/'))
		);

		_ = await Verify(result)
			.UseParameters(framework);
	}

	public static TheoryData<string> Frameworks =>
		[Utility.ReferenceAssemblies.TargetFramework];
}
