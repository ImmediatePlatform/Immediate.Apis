using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Immediate.Apis.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class InvalidRouteGroupNameAnalyzer : DiagnosticAnalyzer
{
	public static readonly DiagnosticDescriptor InvalidRouteGroupName =
		new(
			id: DiagnosticIds.IAPI0010InvalidRouteGroupName,
			title: "Invalid route group name",
			messageFormat: "Route group name `{0}` is not a valid identifier part",
			category: "ImmediateApis",
			defaultSeverity: DiagnosticSeverity.Error,
			isEnabledByDefault: true,
			description: "Route group names must be non-null, non-empty, and only contain characters that are valid past the start of an identifier (letters, digits, underscores, etc.)."
		);

	public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
		ImmutableArray.Create(
		[
			InvalidRouteGroupName,
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

		var attributes = namedTypeSymbol.GetAttributes();

		token.ThrowIfCancellationRequested();

		if (attributes.FirstOrDefault(a => a.AttributeClass.IsRouteGroupAttribute()) is not
			{
				ConstructorArguments: [{ Value: var argumentValue }],
			} routeGroupAttribute)
		{
			return;
		}

		token.ThrowIfCancellationRequested();

		if (RouteGroupUtility.IsValidRouteGroupName(argumentValue as string))
			return;

		token.ThrowIfCancellationRequested();

		context.ReportDiagnostic(
			Diagnostic.Create(
				InvalidRouteGroupName,
				routeGroupAttribute.ApplicationSyntaxReference
					?.GetSyntax(token)
					?.GetLocation(),
				routeGroupAttribute.ConstructorArguments[0].Value
			)
		);
	}
}

