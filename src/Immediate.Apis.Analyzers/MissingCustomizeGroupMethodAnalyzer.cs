using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Immediate.Apis.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class MissingCustomizeGroupMethodAnalyzer : DiagnosticAnalyzer
{
	public static readonly DiagnosticDescriptor MissingCustomizeGroupMethod =
		new(
			id: DiagnosticIds.IAPI0012MissingCustomizeGroupMethod,
			title: "Missing `CustomizeGroup` method",
			messageFormat: "Missing `CustomizeGroup` method in the class",
			category: "ImmediateApis",
			defaultSeverity: DiagnosticSeverity.Hidden,
			isEnabledByDefault: true,
			description: "A class with `RouteGroup` attribute can have a `CustomizeGroup` method."
		);

	public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
		ImmutableArray.Create(
		[
			MissingCustomizeGroupMethod,
		]);

	public override void Initialize(AnalysisContext context)
	{
		if (context == null)
			throw new ArgumentNullException(nameof(context));

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

		if (!namedTypeSymbol
				.GetAttributes()
				.Any(x => x.AttributeClass.IsRouteGroupAttribute))
		{
			return;
		}

		token.ThrowIfCancellationRequested();

		if (namedTypeSymbol
				.GetMembers()
				.OfType<IMethodSymbol>()
				.Any(ims => ims.Name is "CustomizeGroup"))
		{
			return;
		}

		token.ThrowIfCancellationRequested();

		context.ReportDiagnostic(
			Diagnostic.Create(
				MissingCustomizeGroupMethod,
				namedTypeSymbol.Locations[0]
			)
		);
	}
}
