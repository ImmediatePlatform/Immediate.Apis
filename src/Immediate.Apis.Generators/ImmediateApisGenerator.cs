using Microsoft.CodeAnalysis;

namespace Immediate.Apis.Generators;

[Generator]
public sealed partial class ImmediateApisGenerator : IIncrementalGenerator
{
	public void Initialize(IncrementalGeneratorInitializationContext context)
	{
		var methods = context.SyntaxProvider
			.ForAttributeWithMetadataName(
				"Immediate.Handlers.Shared.HandlerAttribute",
				(_, _) => true,
				TransformMethod
			)
			.Where(m => m != null)
			.Collect()!;

		var assemblyName = context.CompilationProvider
			.Select((cp, _) => cp.AssemblyName!
				.Replace(".", string.Empty)
				.Replace(" ", string.Empty)
				.Trim()
			);

		context.RegisterSourceOutput(
			methods.Combine(assemblyName),
			(spc, m) => RenderMethods(spc, m.Left!, m.Right)
		);
	}
}
