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

	private static void RenderRouteGroup(
		SourceProductionContext context,
		RouteGroup group,
		string assemblyName,
		Template template)
	{
		var token = context.CancellationToken;

		token.ThrowIfCancellationRequested();

		var source = template.Render(new
		{
			Assembly = assemblyName,
			group.Name,
			group.Methods,
			Version = ThisAssembly.InformationalVersion,
		});

		token.ThrowIfCancellationRequested();
		context.AddSource($"IA.RouteGroupBuilder.{group.Name}.g.cs", source);
	}
}
