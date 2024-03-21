using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace Immediate.Apis.Generators;

public sealed partial class ImmediateApisGenerator
{
	private static void RenderMethods(
		SourceProductionContext context,
		ImmutableArray<Method> methods,
		string assemblyName
	)
	{
		if (methods.Length == 0)
			return;

		var token = context.CancellationToken;

		var template = Utility.GetTemplate("Routes");
		token.ThrowIfCancellationRequested();

		var source = template.Render(new
		{
			Assembly = assemblyName,
			Methods = methods,
		});

		token.ThrowIfCancellationRequested();
		context.AddSource("RoutesBuilder.g.cs", source);
	}
}
