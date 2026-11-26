using Microsoft.CodeAnalysis;

namespace Immediate.Apis.Generators;

public sealed partial class ImmediateApisGenerator
{
	private static void RegisterXmlComments(
		IncrementalGeneratorInitializationContext context,
		IncrementalValueProvider<string> rootNamespace
	)
	{
		var providers = context.CompilationProvider.Select((compilation, _) =>
		{
			var openApi = GetOperationModelNamespace(
				compilation,
				"Microsoft.AspNetCore.OpenApi.IOpenApiOperationTransformer",
				"TransformAsync"
			);
			var swashbuckle = GetOperationModelNamespace(
				compilation,
				"Swashbuckle.AspNetCore.SwaggerGen.IOperationFilter",
				"Apply"
			);

			return new XmlCommentProviders(openApi, swashbuckle);
		});

		var template = Utility.GetTemplate("XmlComments");
		context.RegisterSourceOutput(providers.Combine(rootNamespace), (spc, pair) =>
		{
			var (openApi, swashbuckle) = pair.Left;
			if (openApi is null && swashbuckle is null)
				return;

			var source = template.Render(new
			{
				Namespace = pair.Right,
				ModelNamespace = openApi ?? swashbuckle,
				OpenApi = openApi is not null,
				Swashbuckle = swashbuckle is not null,
				Version = ThisAssembly.InformationalVersion,
			});

			spc.AddSource("IA.XmlComments.g.cs", source);
		});
	}

	private static string? GetOperationModelNamespace(Compilation compilation, string provider, string method)
	{
		var providerMethod = compilation.GetTypeByMetadataName(provider)?.GetMembers(method).OfType<IMethodSymbol>().FirstOrDefault();
		return providerMethod?.Parameters is [{ Type: { Name: "OpenApiOperation", TypeKind: not TypeKind.Error } operation }, ..]
			? operation.ContainingNamespace.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)
			: null;
	}

	private sealed record XmlCommentProviders(string? OpenApi, string? Swashbuckle);
}
