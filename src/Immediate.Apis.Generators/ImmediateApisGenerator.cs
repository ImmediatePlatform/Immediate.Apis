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
			.WhereNotNull()
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

		var allGroups = methods
			.Collect()
			.SelectMany(
				(g, _) => g
					.GroupBy(
						m => m.RouteGroupName,
						(k, g) => new RouteGroup { Name = k, Methods = g.ToEquatableReadOnlyList() },
						StringComparer.Ordinal
					)
			)
			.Where(x => x.Name is null || RouteGroupUtility.IsValidRouteGroupName(x.Name))
			.WithTrackingName("GroupedMethods");

		var allMethodsTemplate = Utility.GetTemplate("Routes");
		context.RegisterSourceOutput(
			allGroups.Combine(assemblyName),
			(spc, m) => RenderRouteGroup(spc, m.Left, m.Right, allMethodsTemplate)
		);
	}
}
