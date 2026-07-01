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

		var assemblyName = context.CompilationProvider
			.Select((cp, _) => cp.GetAssemblyIdentifier())
			.WithTrackingName("AssemblyName");

		var perMethodTemplate = Utility.GetTemplate("Route");
		context.RegisterSourceOutput(
			endpoints.Combine(assemblyName),
			(spc, m) => RenderEndpoint(spc, m.Left, m.Right, perMethodTemplate)
		);

		var routesByGroup = endpoints
			.GroupBy(
				m => ValueTuple.Create(m.RouteGroupClassFullName),
				m => new RouteEndpoint { Name = m.Class.Name, ClassFullName = m.ClassFullName, Routes = m.Routes }
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
				.Combine(assemblyName),
			(spc, m) => RenderMapEndpoints(spc, m.Left.Left, m.Left.Right, m.Right, mapEndpointsTemplate)
		);

		var routeGroups = routesByGroup.Combine(routeGroupsByGroup)
			.SelectMany(BuildRouteGroups)
			.WithTrackingName("RouteGroups");

		var routeGroupTemplate = Utility.GetTemplate("RouteGroup");
		context.RegisterSourceOutput(
			routeGroups.Combine(assemblyName),
			(spc, m) => RenderRouteGroup(spc, m.Left, m.Right, routeGroupTemplate)
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
				Endpoints = endpoints.GetValueOrDefault(ValueTuple.Create(g.ClassFullName)),
				Groups = BuildRouteGroups(endpoints, routeGroups, g.ClassFullName, token).ToEquatableReadOnlyList(),
			};
		}
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
			.Replace(".", string.Empty)
			.Replace(" ", string.Empty)
			.Trim();
	}
}
