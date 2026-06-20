using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Scriban;

namespace Immediate.Apis.Generators;

public sealed partial class ImmediateApisGenerator
{
	private static void RenderMethod(
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
		context.AddSource($"IA.RouteBuilder.{method.ClassAsMethodName}.g.cs", source);
	}

	private static void RenderMethods(
		SourceProductionContext context,
		ImmutableArray<Method> methods,
		string assemblyName,
		Template template
	)
	{
		if (methods.Length == 0)
			return;

		var token = context.CancellationToken;

		token.ThrowIfCancellationRequested();

		var source = template.Render(new
		{
			Assembly = assemblyName,
			Methods = methods,
			Version = ThisAssembly.InformationalVersion,
		});

		token.ThrowIfCancellationRequested();
		context.AddSource("IA.RouteGroupBuilder.g.cs", source);
	}
}
