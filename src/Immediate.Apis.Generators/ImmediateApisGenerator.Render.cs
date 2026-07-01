using Microsoft.CodeAnalysis;
using Scriban;

namespace Immediate.Apis.Generators;

public sealed partial class ImmediateApisGenerator
{
	private static void RenderEndpoint(
		SourceProductionContext context,
		Method method,
		string assemblyName,
		Template template
	)
	{
		var token = context.CancellationToken;
		token.ThrowIfCancellationRequested();

		var source = template.Render(new
		{
			Assembly = assemblyName,
			Method = method,
			Version = ThisAssembly.InformationalVersion,
		});

		token.ThrowIfCancellationRequested();

		context.AddSource($"IA.{method.Namespace}.{method.Class.Name}.g.cs", source);
	}

	private static void RenderMapEndpoints(
		SourceProductionContext context,
		EquatableReadOnlyList<RouteEndpoint> endpoints,
		EquatableReadOnlyList<RouteGroupDefinition> groups,
		string assemblyName,
		Template template
	)
	{
		var token = context.CancellationToken;
		token.ThrowIfCancellationRequested();

		var source = template.Render(new
		{
			Assembly = assemblyName,
			Endpoints = endpoints,
			Groups = groups,
			Version = ThisAssembly.InformationalVersion,
		});

		token.ThrowIfCancellationRequested();
		context.AddSource("IA.MapEndpoints.g.cs", source);
	}

	private static void RenderRouteGroup(
		SourceProductionContext context,
		RouteGroup group,
		string assemblyName,
		Template template
	)
	{
		var token = context.CancellationToken;
		token.ThrowIfCancellationRequested();

		var source = template.Render(new
		{
			Assembly = assemblyName,
			Root = group,
			Version = ThisAssembly.InformationalVersion,
		});

		var name = $"{group.Definition.Namespace}.{string.Join(".", group.Definition.OuterClasses.Select(c => c.Name))}.{group.Definition.Class.Name}";
		context.AddSource($"IA.{name}.g.cs", source);
	}
}
