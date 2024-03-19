using System.Collections.Immutable;
using Immediate.Handlers.Analyzers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Immediate.Apis.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class MissingHandlerAttributeAnalyzer : DiagnosticAnalyzer
{
	public static readonly DiagnosticDescriptor MissingHandlerAttribute =
		new(
			id: DiagnosticIds.IAPI0001MissingHandlerAttribute,
			title: "[Handler] must be used",
			messageFormat: "Handler `{0}` must be marked with [Handler]",
			category: "ImmediateApis",
			defaultSeverity: DiagnosticSeverity.Error,
			isEnabledByDefault: true,
			description: "An endpoint registration can only be generated for an Immediate.Handlers handler."
		);

	public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
		ImmutableArray.Create(
		[
			MissingHandlerAttribute,
		]);

	public override void Initialize(AnalysisContext context)
	{
		if (context == null)
			throw new ArgumentNullException(nameof(context));

		context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
		context.EnableConcurrentExecution();

		context.RegisterSymbolAction(AnalyzeSymbol, SymbolKind.NamedType);
	}

	private static readonly string[] s_validAttributes =
		[
			"Immediate.Apis.Shared.MapGetAttribute",
			"Immediate.Apis.Shared.MapPostAttribute",
			"Immediate.Apis.Shared.MapPutAttribute",
			"Immediate.Apis.Shared.MapPatchAttribute",
			"Immediate.Apis.Shared.MapDeleteAttribute",
		];

	private static void AnalyzeSymbol(SymbolAnalysisContext context)
	{
		var token = context.CancellationToken;
		token.ThrowIfCancellationRequested();

		if (context.Symbol is not INamedTypeSymbol namedTypeSymbol)
			return;

		if (!namedTypeSymbol
				.GetAttributes()
				.Any(x => s_validAttributes.Contains(x.AttributeClass?.ToString()))
		)
		{
			return;
		}

		token.ThrowIfCancellationRequested();

		if (!namedTypeSymbol
				.GetAttributes()
				.Any(x => x.AttributeClass?.ToString() == "Immediate.Handlers.Shared.HandlerAttribute")
		)
		{
			context.ReportDiagnostic(
				Diagnostic.Create(
					MissingHandlerAttribute,
					namedTypeSymbol.Locations[0],
					namedTypeSymbol.Name
				)
			);
		}
	}
}
