using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Immediate.Apis.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class CustomizeEndpointUsageAnalyzer : DiagnosticAnalyzer
{
	public static readonly DiagnosticDescriptor MustBeValidDefinition =
		new(
			id: DiagnosticIds.IAPI0004CustomizeEndpointInvalid,
			title: "`CustomizeEndpoint` requires a specific definition",
			messageFormat: "`CustomizeEndpoint` must be `internal static void CustomizeEndpoint(IEndpointConventionBuilder endpoint)`",
			category: "ImmediateApis",
			defaultSeverity: DiagnosticSeverity.Warning,
			isEnabledByDefault: true,
			description: "An invalid definition of `CustomizeEndpoint` will not be used in the registration."
		);

	public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
		ImmutableArray.Create(
		[
			MustBeValidDefinition,
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
				.Any(x => x.AttributeClass.IsMapMethodAttribute()))
		{
			return;
		}

		token.ThrowIfCancellationRequested();

		if (namedTypeSymbol
			.GetMembers()
			.OfType<IMethodSymbol>()
			.Where(ims => ims.Name is "CustomizeEndpoint")
			.ToList() is not [{ } customizeEndpointMethod])
		{
			return;
		}

		if (customizeEndpointMethod is
			{
				DeclaredAccessibility: Accessibility.Internal,
				IsStatic: true,
				ReturnsVoid: true,
				Parameters: [{ Type: { } paramType }],
			}
			&& paramType.IsIEndpointConventionBuilder())
		{
			return;
		}

		var syntax = (MethodDeclarationSyntax)customizeEndpointMethod
			.DeclaringSyntaxReferences[0]
			.GetSyntax();

		context.ReportDiagnostic(
			Diagnostic.Create(
				MustBeValidDefinition,
				syntax
					.Identifier
					.GetLocation()
			)
		);
	}
}
