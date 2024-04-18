using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Immediate.Apis.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class MissingTransformResultMethodAnalyzer : DiagnosticAnalyzer
{
	public static readonly DiagnosticDescriptor MissingTransformResultMethod =
		new(
			id: DiagnosticIds.IAPI0007MissingTransformResultMethod,
			title: "Missing `TransformResult` method",
			messageFormat: "Missing `TransformResult` method in the class",
			category: "ImmediateApis",
			defaultSeverity: DiagnosticSeverity.Hidden,
			isEnabledByDefault: true,
			description: "A class with `MapMethod` attribute can have a `TransformResult` method."
		);

	public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
		ImmutableArray.Create(
		[
			MissingTransformResultMethod,
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
				.All(ims => ims.Name is not "Handle"))
		{
			return;
		}

		if (namedTypeSymbol
				.GetMembers()
				.OfType<IMethodSymbol>()
				.Any(ims => ims.Name is "TransformResult"))
		{
			return;
		}

		token.ThrowIfCancellationRequested();

		var syntax = (ClassDeclarationSyntax)namedTypeSymbol
			.DeclaringSyntaxReferences[0]
			.GetSyntax();

		context.ReportDiagnostic(
			Diagnostic.Create(
				MissingTransformResultMethod,
				syntax
					.Identifier
					.GetLocation()
			)
		);
	}
}
