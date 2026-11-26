#if NET10_0_OR_GREATER
using Microsoft.AspNetCore.OpenApi;
#endif
using Microsoft.CodeAnalysis;
using Microsoft.OpenApi;
#if !NET10_0
using Swashbuckle.AspNetCore.SwaggerGen;
#endif
using static Immediate.Apis.Tests.Utility;

namespace Immediate.Apis.Tests.GeneratorTests;

public sealed class XmlCommentsAdapterTests
{
#if NET10_0_OR_GREATER
	[Fact]
	public async Task EmitsOnlyAspNetCoreAdapterWhenReferenced()
	{
		var result = GenerateAdapter(
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

		_ = await VerifyIgnoreImmediateHandlers(result);
	}
#endif

#if !NET10_0
	[Fact]
	public async Task EmitsOnlySwashbuckleAdapterWhenReferenced()
	{
		var result = GenerateAdapter(
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

		_ = await VerifyIgnoreImmediateHandlers(result);
	}
#endif

#if NET11_0
	[Fact]
	public async Task EmitsBothAdaptersWithCompatibleProviderReferences()
	{
		var result = GenerateAdapter(
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

		_ = await VerifyIgnoreImmediateHandlers(result);
	}
#endif

	private static GeneratorDriverRunResult GenerateAdapter(string source, IReadOnlyList<Type> providers) =>
		GeneratorTestHelper.RunGenerator(
			source,
			additionalReferences:
			[
				MetadataReference.CreateFromFile(typeof(OpenApiOperation).Assembly.Location),
				..providers.Select(p => MetadataReference.CreateFromFile(p.Assembly.Location)),
			],
			skippedSteps: ["Endpoints"]
		);
}
