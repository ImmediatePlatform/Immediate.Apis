using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Immediate.Apis.Generators;

[Generator]
public sealed partial class ImmediateApisGenerator : IIncrementalGenerator
{
	public void Initialize(IncrementalGeneratorInitializationContext context)
	{
		var endpoints = context.SyntaxProvider
			.ForAttributeWithMetadataName(
				"Immediate.Handlers.Shared.HandlerAttribute",
				predicate: (node, _) => node is TypeDeclarationSyntax,
				transform: TransformEndpoint
			)
			.WhereNotNull()
			.WithTrackingName("Endpoints");

		var assemblyDefaults = context.CompilationProvider
			.Select((cp, _) => new AssemblyDefaults
			{
				AssemblyName = cp.GetAssemblyIdentifier(),
				LanguageVersion = (cp.SyntaxTrees.FirstOrDefault()?.Options as CSharpParseOptions)?.LanguageVersion ?? LanguageVersion.LatestMajor,
			})
			.WithTrackingName("AssemblyName");

		var perMethodTemplate = Utility.GetTemplate("Route");
		context.RegisterSourceOutput(
			endpoints,
			(spc, m) => RenderEndpoint(spc, m, perMethodTemplate)
		);

		var routesByGroup = endpoints
			.GroupBy(
				m => ValueTuple.Create(m.RouteGroupClassFullName),
				m => new RouteEndpoint { Name = m.Class.Name, ClassFullName = m.ClassFullName, Tags = m.Tags, Routes = m.Routes }
			)
			.WithTrackingName("EndpointsDictionary");

		var routeGroupsByGroup = context.SyntaxProvider
			.ForAttributeWithMetadataName(
				"Immediate.Apis.Shared.RouteGroupAttribute",
				predicate: (node, _) => node is TypeDeclarationSyntax,
				transform: TransformRouteGroup
			)
			.WhereNotNull()
			.GroupBy(rg => ValueTuple.Create(rg.RouteGroupClassFullName))
			.WithTrackingName("RouteGroupsDictionary");

		var mapEndpointsTemplate = Utility.GetTemplate("MapEndpoints");
		context.RegisterSourceOutput(
			routesByGroup.Select((g, _) => g.GetValueOrDefault(ValueTuple.Create<string?>(item1: null)))
				.Combine(routeGroupsByGroup.Select((g, _) => g.GetValueOrDefault(ValueTuple.Create<string?>(item1: null))))
				.Combine(assemblyDefaults),
			(spc, m) => RenderMapEndpoints(spc, m.Left.Left, m.Left.Right, m.Right, mapEndpointsTemplate)
		);

		var routeGroups = routesByGroup.Combine(routeGroupsByGroup)
			.SelectMany(BuildRouteGroups)
			.WithTrackingName("RouteGroups");

		var routeGroupTemplate = Utility.GetTemplate("RouteGroup");
		context.RegisterSourceOutput(
			routeGroups,
			(spc, rg) => RenderRouteGroup(spc, rg, routeGroupTemplate)
		);
	}

	private static IEnumerable<RouteGroup> BuildRouteGroups(
		(
			EquatableDictionary<ValueTuple<string?>, EquatableReadOnlyList<RouteEndpoint>> Left,
			EquatableDictionary<ValueTuple<string?>, EquatableReadOnlyList<RouteGroupDefinition>> Right
		) tuple,
		CancellationToken token
	) => BuildRouteGroups(tuple.Left, tuple.Right, classFullName: null, token);

	private static IEnumerable<RouteGroup> BuildRouteGroups(
		EquatableDictionary<ValueTuple<string?>, EquatableReadOnlyList<RouteEndpoint>> endpoints,
		EquatableDictionary<ValueTuple<string?>, EquatableReadOnlyList<RouteGroupDefinition>> routeGroups,
		string? classFullName,
		CancellationToken token
	)
	{
		foreach (var g in routeGroups.GetValueOrDefault(ValueTuple.Create(classFullName)).OrderBy(x => x.ClassFullName, StringComparer.Ordinal))
		{
			token.ThrowIfCancellationRequested();

			yield return new()
			{
				Definition = g,
				Tags = BuildTags(
					endpoints.GetValueOrDefault(ValueTuple.Create(g.ClassFullName)),
					[.. BuildRouteGroups(endpoints, routeGroups, g.ClassFullName, token)]
				),
			};
		}
	}

	private static EquatableReadOnlyList<RouteTag> BuildTags(
		IReadOnlyList<RouteEndpoint> endpoints,
		IReadOnlyList<RouteGroup> routeGroups
	)
	{
		var endpointLookup = endpoints.ToLookup(e => e.Tags, StringComparer.Ordinal);
		var groupLookup = routeGroups.ToLookup(e => e.Definition.Tags, StringComparer.Ordinal);

		return endpointLookup.Select(l => l.Key)
			.Union(groupLookup.Select(l => l.Key), StringComparer.Ordinal)
			.OrderBy(l => l, StringComparer.Ordinal)
			.Select(k => new RouteTag
			{
				Tag = k,
				Endpoints = endpointLookup[k].ToEquatableReadOnlyList(),
				Groups = groupLookup[k].ToEquatableReadOnlyList(),
			})
			.ToEquatableReadOnlyList();
	}
}

file static class Extensions
{
	public static string GetAssemblyIdentifier(this Compilation compilation)
	{
		if (compilation.Assembly.GetAttributes()
				.FirstOrDefault(a => a.AttributeClass.IsImmediateAssemblyIdentifierAttribute)
				is { ConstructorArguments: [{ Value: string { Length: >= 1 } identifier }] }
			&& identifier[0] != '@'
			&& SyntaxFacts.IsValidIdentifier(identifier))
		{
			return identifier;
		}

		return compilation.AssemblyName!
			.Replace(".", string.Empty, StringComparison.Ordinal)
			.Replace(" ", string.Empty, StringComparison.Ordinal)
			.Replace("-", string.Empty, StringComparison.Ordinal)
			.Trim();
	}
}
