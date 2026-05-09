using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Immediate.Apis.Generators;

[Generator]
public sealed partial class ImmediateApisGenerator : IIncrementalGenerator
{
	public void Initialize(IncrementalGeneratorInitializationContext context)
	{
		var methods = context.SyntaxProvider
			.ForAttributeWithMetadataName(
				"Immediate.Handlers.Shared.HandlerAttribute",
				predicate: (node, _) => node is TypeDeclarationSyntax,
				transform: TransformMethod
			)
			.Where(m => m != null)
			.WithTrackingName("Handlers");

		var assemblyName = context.CompilationProvider
			.Select((cp, _) => cp.AssemblyName!
				.Replace(".", string.Empty)
				.Replace(" ", string.Empty)
				.Trim()
			)
			.WithTrackingName("AssemblyName");

		var perMethodTemplate = Utility.GetTemplate("Route");
		context.RegisterSourceOutput(
			methods.Combine(assemblyName),
			(spc, m) => RenderMethod(spc, m.Left!, m.Right, perMethodTemplate)
		);

		var ungroupedMethodsTemplate = Utility.GetTemplate("Routes");
		context.RegisterSourceOutput(
			methods
				.Where(m => !m!.HasRouteGroup)
				.WithTrackingName("UngroupedMethods")
				.Collect()
				.Combine(assemblyName),
			(spc, m) => RenderMethods(spc, m.Left!, m.Right, ungroupedMethodsTemplate)
		);

		var groupedMethodsTemplate = Utility.GetTemplate("RouteGroup");
		var methodGroups = methods
			.Where(m => RouteGroupUtility.IsValidRouteGroupName(m!.RouteGroupName))
			.WithTrackingName("GroupedMethods")
			.Collect()
			.SelectMany((methods, _) =>
				methods
					.GroupBy(m => m!.RouteGroupName, StringComparer.Ordinal)
					.Select(g => new RouteGroup { Name = g.Key!, Methods = g.ToEquatableReadOnlyList() })
			)
			.WithTrackingName("RouteGroups");

		context.RegisterSourceOutput(
			methodGroups.Combine(assemblyName),
			(spc, m) => RenderRouteGroup(spc, m.Left, m.Right, groupedMethodsTemplate)
		);
	}
}
