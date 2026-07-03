using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Immediate.Apis.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class CustomizeEndpointUsageAnalyzer : DiagnosticAnalyzer
{
	public static readonly DiagnosticDescriptor CustomizeEndpointInvalid =
		new(
			id: DiagnosticIds.IAPI0004CustomizeEndpointInvalid,
			title: "`CustomizeEndpoint` requires a specific definition",
			messageFormat: "`CustomizeEndpoint` must be `internal static void CustomizeEndpoint(RouteHandlerBuilder endpoint)`",
			category: "ImmediateApis",
			defaultSeverity: DiagnosticSeverity.Warning,
			isEnabledByDefault: true,
			description: "An invalid definition of `CustomizeEndpoint` will not be used in the registration.",
			customTags: [WellKnownDiagnosticTags.Unnecessary]
		);

	public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
		ImmutableArray.Create(
		[
			CustomizeEndpointInvalid,
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
				.Any(x => x.AttributeClass.IsMapMethodAttribute))
		{
			return;
		}

		token.ThrowIfCancellationRequested();

		var methods = namedTypeSymbol
			.GetMembers()
			.OfType<IMethodSymbol>()
			.Where(ims => ims.Name is "CustomizeEndpoint")
			.ToList();

		foreach (var customizeEndpointMethod in methods)
		{
			if (methods.Count == 1
				&& customizeEndpointMethod is
				{
					DeclaredAccessibility: Accessibility.Internal or Accessibility.Private,
					IsStatic: true,
					ReturnsVoid: true,
					Parameters: [{ Type.IsIEndpointConventionBuilderOrRouteHandlerBuilder: true }],
				})
			{
				return;
			}

			context.ReportDiagnostic(
				Diagnostic.Create(
					CustomizeEndpointInvalid,
					customizeEndpointMethod.Locations[0]
				)
			);
		}
	}
}
