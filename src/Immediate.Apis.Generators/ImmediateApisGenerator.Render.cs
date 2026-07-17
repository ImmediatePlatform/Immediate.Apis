using Microsoft.CodeAnalysis;
using Scriban;

namespace Immediate.Apis.Generators;

public sealed partial class ImmediateApisGenerator
{
	private static void RenderEndpoint(
		SourceProductionContext context,
		Method method,
		Template template
	)
	{
		var token = context.CancellationToken;
		token.ThrowIfCancellationRequested();

		var source = template.Render(new
		{
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
		AssemblyDefaults assemblyDefaults,
		Template template
	)
	{
		var token = context.CancellationToken;
		token.ThrowIfCancellationRequested();

		var endpointLookup = endpoints.ToLookup(e => e.Tags, StringComparer.Ordinal);
		var groupLookup = groups.ToLookup(e => e.Tags, StringComparer.Ordinal);

		var tags = endpointLookup.Select(l => l.Key)
			.Union(groupLookup.Select(l => l.Key), StringComparer.Ordinal)
			.OrderBy(l => l, StringComparer.Ordinal)
			.Select(k => new
			{
				Tag = k,
				Endpoints = endpointLookup[k].ToEquatableReadOnlyList(),
				Groups = groupLookup[k].ToEquatableReadOnlyList(),
			})
			.ToEquatableReadOnlyList();

		var source = template.Render(new
		{
			assemblyDefaults.AssemblyName,
			assemblyDefaults.LanguageVersion,

			Tags = tags,
			Version = ThisAssembly.InformationalVersion,
		});

		token.ThrowIfCancellationRequested();
		context.AddSource("IA.MapEndpoints.g.cs", source);
	}

	private static void RenderRouteGroup(
		SourceProductionContext context,
		RouteGroup group,
		Template template
	)
	{
		var token = context.CancellationToken;
		token.ThrowIfCancellationRequested();

		var source = template.Render(new
		{
			Root = group,
			Version = ThisAssembly.InformationalVersion,
		});

		var name = $"{group.Definition.Namespace}.{string.Join(".", group.Definition.OuterClasses.Select(c => c.Name))}.{group.Definition.Class.Name}";
		context.AddSource($"IA.{name}.g.cs", source);
	}
}
