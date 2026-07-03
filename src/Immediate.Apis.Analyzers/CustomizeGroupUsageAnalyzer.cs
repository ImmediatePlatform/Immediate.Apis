using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Immediate.Apis.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class CustomizeGroupUsageAnalyzer : DiagnosticAnalyzer
{
	public static readonly DiagnosticDescriptor CustomizeGroupInvalid =
		new(
			id: DiagnosticIds.IAPI0011CustomizeGroupInvalid,
			title: "`CustomizeGroup` requires a specific definition",
			messageFormat: "`CustomizeGroup` must be `private static void CustomizeEndpoint(RouteGroupBuilder group)`",
			category: "ImmediateApis",
			defaultSeverity: DiagnosticSeverity.Warning,
			isEnabledByDefault: true,
			description: "An invalid definition of `CustomizeGroup` will not be used in the registration.",
			customTags: [WellKnownDiagnosticTags.Unnecessary]
		);

	public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
		ImmutableArray.Create(
		[
			CustomizeGroupInvalid,
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

		if (!namedTypeSymbol
				.GetAttributes()
				.Any(x => x.AttributeClass.IsRouteGroupAttribute))
		{
			return;
		}

		token.ThrowIfCancellationRequested();

		var methods = namedTypeSymbol
			.GetMembers()
			.OfType<IMethodSymbol>()
			.Where(ims => ims.Name is "CustomizeGroup")
			.ToList();

		foreach (var customizeGroupMethod in methods)
		{
			if (methods.Count == 1
				&& customizeGroupMethod is
				{
					DeclaredAccessibility: Accessibility.Private,
					IsStatic: true,
					ReturnsVoid: true,
					Parameters: [{ Type.IsRouteGroupBuilder: true }],
				})
			{
				return;
			}

			context.ReportDiagnostic(
				Diagnostic.Create(
					CustomizeGroupInvalid,
					customizeGroupMethod.Locations[0]
				)
			);
		}
	}
}
