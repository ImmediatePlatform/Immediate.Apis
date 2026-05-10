using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Immediate.Apis.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class DuplicateEndpointAnalyzer : DiagnosticAnalyzer
{
	public static readonly DiagnosticDescriptor EndpointHasBeenSpecifiedMultipleTimes =
		new(
			id: DiagnosticIds.IAPI0009EndpointHasBeenSpecifiedMultipleTimes,
			title: "Endpoint has been specified multiple times",
			messageFormat: "Endpoint `{0} {1}` {2}has been specified multiple times: {3}",
			category: "ImmediateApis",
			defaultSeverity: DiagnosticSeverity.Warning,
			isEnabledByDefault: true,
			description: "Duplicate endpoints may lead to confusion on which endpoint will be registered.",
			customTags: [WellKnownDiagnosticTags.CompilationEnd]
		);

	public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
		ImmutableArray.Create(
		[
			EndpointHasBeenSpecifiedMultipleTimes,
		]);

	public override void Initialize(AnalysisContext context)
	{
		ArgumentNullException.ThrowIfNull(context);

		context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
		context.EnableConcurrentExecution();

		context.RegisterCompilationStartAction(context =>
		{
			var endpoints = new List<(string Verb, string Route, string ClassName, AttributeData AttributeData, string? RouteGroup)>();
			var @lock = new Lock();

			context.RegisterSymbolAction(
				context =>
				{
					var attributes = context.Symbol.GetAttributes();
					if (
						attributes.GetMethodAttribute() is { } attribute
						&& attribute.GetHttpMethod() is { } method
						&& attribute.GetRoutes() is { Count: > 0 } routes
					)
					{
						lock (@lock)
						{
							endpoints.AddRange(
								routes
									.Select(r => (
										method.ToUpperInvariant(),
										r,
										context.Symbol.Name,
										attribute,
										attributes.GetRouteGroupAttribute()?.GetRouteGroup()
									))
							);
						}
					}
				},
				SymbolKind.NamedType
			);

			context.RegisterCompilationEndAction(
				context =>
				{
					foreach (var endpointGroup in endpoints.GroupBy(x => (x.Verb, x.Route, x.RouteGroup)).Where(g => g.Skip(1).Any()))
					{
						var classes = string.Join(", ", endpointGroup.Select(l => l.ClassName));

						foreach (var location in endpointGroup)
						{
							context.ReportDiagnostic(
								Diagnostic.Create(
									EndpointHasBeenSpecifiedMultipleTimes,
									location.AttributeData.ApplicationSyntaxReference?.GetSyntax(context.CancellationToken).GetLocation(),
									endpointGroup.Key.Verb,
									endpointGroup.Key.Route,
									endpointGroup.Key.RouteGroup is not null ? $"in route group `{endpointGroup.Key.RouteGroup}` " : string.Empty,
									classes
								)
							);
						}
					}
				}
			);
		});
	}
}
