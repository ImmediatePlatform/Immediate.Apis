using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
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
			.Select((cp, _) => cp.GetAssemblyIdentifier())
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
