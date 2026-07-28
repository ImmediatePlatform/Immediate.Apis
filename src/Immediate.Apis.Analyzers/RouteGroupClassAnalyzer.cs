using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Immediate.Apis.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class RouteGroupClassAnalyzer : DiagnosticAnalyzer
{
	public static readonly DiagnosticDescriptor RouteGroupClassMustNotBeNested =
		new(
			id: DiagnosticIds.IAPI0014RouteGroupClassMustNotBeNested,
			title: "Route Group nesting is not allowed",
			messageFormat: "Route Group '{0}' must not be nested in another type that is not also a route group",
			category: "ImmediateApis",
			defaultSeverity: DiagnosticSeverity.Error,
			isEnabledByDefault: true,
			description: "Route groups are not supported nested in other classes.",
			customTags: [WellKnownDiagnosticTags.NotConfigurable]
		);

	public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
		ImmutableArray.Create(
		[
			RouteGroupClassMustNotBeNested,
		]);

	public override void Initialize(AnalysisContext context)
	{
		ArgumentNullException.ThrowIfNull(context);

		context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
		context.EnableConcurrentExecution();

		context.RegisterSymbolAction(AnalyzeSymbol, SymbolKind.NamedType);
	}

	private static void AnalyzeSymbol(SymbolAnalysisContext context)
	{
		var token = context.CancellationToken;
		token.ThrowIfCancellationRequested();

		if (context.Symbol is not INamedTypeSymbol namedTypeSymbol)
			return;

		if (namedTypeSymbol.GetAttributes().GetRouteGroupAttribute() is null)
			return;

		if (namedTypeSymbol.ContainingType is null)
			return;

		if (namedTypeSymbol.ContainingType.GetAttributes().GetRouteGroupAttribute() is { })
			return;

		context.ReportDiagnostic(
			Diagnostic.Create(
				RouteGroupClassMustNotBeNested,
				namedTypeSymbol.Locations.FirstOrDefault(),
				namedTypeSymbol.ToDisplayString(SymbolDisplayFormat.CSharpShortErrorMessageFormat)
			)
		);
	}
}
