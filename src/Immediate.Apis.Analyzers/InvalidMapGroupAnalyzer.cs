using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Immediate.Apis.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class InvalidMapGroupAnalyzer : DiagnosticAnalyzer
{
	public static readonly DiagnosticDescriptor InvalidMapGroupTarget =
		new(
			id: DiagnosticIds.IAPI0013InvalidMapGroupTarget,
			title: "MapGroup<> target must be a valid Route Group",
			messageFormat: "Target `{0}` in `[MapGroup<{0}>]` must be a Route Group",
			category: "ImmediateApis",
			defaultSeverity: DiagnosticSeverity.Error,
			isEnabledByDefault: true,
			description: "Invalid route group specification means the endpoint won't be registered.",
			customTags: [WellKnownDiagnosticTags.Unnecessary, WellKnownDiagnosticTags.NotConfigurable]
		);

	public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
		ImmutableArray.Create(
		[
			InvalidMapGroupTarget,
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

		var attributes = namedTypeSymbol.GetAttributes();

		if (!attributes.Any(a => a.AttributeClass.IsMapMethodAttribute))
			return;

		token.ThrowIfCancellationRequested();

		if (attributes.FirstOrDefault(a => a.AttributeClass.IsMapGroupAttribute) is not { AttributeClass.TypeArguments: [{ } targetSymbol] } attribute)
			return;

		if (targetSymbol.GetAttributes().GetRouteGroupAttribute() is { })
			return;

		context.ReportDiagnostic(
			Diagnostic.Create(
				InvalidMapGroupTarget,
				attribute.ApplicationSyntaxReference?.GetSyntax(token).GetLocation(),
				targetSymbol.ToDisplayString(SymbolDisplayFormat.CSharpShortErrorMessageFormat)
			)
		);
	}
}
