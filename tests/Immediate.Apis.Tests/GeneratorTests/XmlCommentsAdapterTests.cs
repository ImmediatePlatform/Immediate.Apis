#if NET10_0_OR_GREATER
using Microsoft.AspNetCore.OpenApi;
#endif
using Microsoft.CodeAnalysis;
using Microsoft.OpenApi;
#if !NET10_0
using Swashbuckle.AspNetCore.SwaggerGen;
#endif

namespace Immediate.Apis.Tests.GeneratorTests;

public sealed class XmlCommentsAdapterTests
{
#if NET10_0_OR_GREATER
	[Fact]
	public void EmitsOnlyAspNetCoreAdapterWhenReferenced()
	{
		var source = GenerateAdapter(
			"""
			using Microsoft.AspNetCore.OpenApi;

			namespace Immediate.Apis.Testing;

			public static class ConfigureDocument
			{
				public static OpenApiOptions Configure(OpenApiOptions options) => options.AddImmediateApisXmlComments();
			}
			""",
			[typeof(OpenApiOptions)]
		);

		Assert.Contains("options.AddOperationTransformer(", source, StringComparison.Ordinal);
		Assert.DoesNotContain("Swashbuckle", source, StringComparison.Ordinal);
	}
#endif

#if !NET10_0
	[Fact]
	public void EmitsOnlySwashbuckleAdapterWhenReferenced()
	{
		var source = GenerateAdapter(
			"""
			using Swashbuckle.AspNetCore.SwaggerGen;

			namespace Immediate.Apis.Testing;

			public static class ConfigureDocument
			{
				public static SwaggerGenOptions Configure(SwaggerGenOptions options) => options.AddImmediateApisXmlComments();
			}
			""",
			[typeof(SwaggerGenOptions)]
		);

		Assert.Contains(".AddOperationFilterInstance(", source, StringComparison.Ordinal);
		Assert.DoesNotContain("Microsoft.AspNetCore.OpenApi", source, StringComparison.Ordinal);
	}
#endif

#if NET11_0
	[Fact]
	public void EmitsBothAdaptersWithCompatibleProviderReferences()
	{
		var source = GenerateAdapter(
			"""
			using Microsoft.AspNetCore.OpenApi;
			using Swashbuckle.AspNetCore.SwaggerGen;

			namespace Immediate.Apis.Testing;

			public static class ConfigureDocument
			{
				public static OpenApiOptions Configure(OpenApiOptions options) => options.AddImmediateApisXmlComments();
				public static SwaggerGenOptions Configure(SwaggerGenOptions options) => options.AddImmediateApisXmlComments();
			}
			""",
			[typeof(OpenApiOptions), typeof(SwaggerGenOptions)]
		);

		Assert.Contains("options.AddOperationTransformer(", source, StringComparison.Ordinal);
		Assert.Contains(".AddOperationFilterInstance(", source, StringComparison.Ordinal);
	}
#endif

	private static string GenerateAdapter(string source, IReadOnlyList<Type> providers)
	{
		var result = GeneratorTestHelper.RunGenerator(
			source,
			additionalReferences:
			[
				MetadataReference.CreateFromFile(typeof(OpenApiOperation).Assembly.Location),
				..providers.Select(p => MetadataReference.CreateFromFile(p.Assembly.Location)),
			],
			skippedSteps: ["Endpoints"]
		);
		return Assert.Single(result.Results[0].GeneratedSources, s => s.HintName is "IA.XmlComments.g.cs").SourceText.ToString();
	}
}
