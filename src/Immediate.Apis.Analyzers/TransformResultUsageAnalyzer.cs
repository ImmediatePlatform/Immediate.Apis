using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Immediate.Apis.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class TransformResultUsageAnalyzer : DiagnosticAnalyzer
{
	public static readonly DiagnosticDescriptor MustBeValidDefinition =
		new(
			id: DiagnosticIds.IAPI0005TransformResultInvalid,
			title: "`TransformResult` requires a specific definition",
			messageFormat: "`TransformResult` must be `internal` and `static`, and accept the value that `HandleAsync` returns",
			category: "ImmediateApis",
			defaultSeverity: DiagnosticSeverity.Warning,
			isEnabledByDefault: true,
			description: "An invalid definition of `TransformResult` will not be used in the registration."
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
			.Where(ims => ims.Name is "TransformResult")
			.ToList() is not [{ } transformResultMethod])
		{
			return;
		}

		if (IsValidTransformMethod(transformResultMethod, namedTypeSymbol))
		{
			return;
		}

		var syntax = (MethodDeclarationSyntax)transformResultMethod
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

	private static bool IsValidTransformMethod(IMethodSymbol transformResultMethod, INamedTypeSymbol namedTypeSymbol)
	{
		if (transformResultMethod is not
			{
				DeclaredAccessibility: Accessibility.Internal,
				IsStatic: true,
				ReturnsVoid: false,
				Parameters: [{ Type: { } paramType }],
			})
		{
			return false;
		}

		if (namedTypeSymbol
			.GetMembers()
			.OfType<IMethodSymbol>()
			.Where(ims => ims is
			{
				Name: "Handle" or "HandleAsync",
				IsStatic: true,
			})
			.ToList() is not [
			{
				ReturnsVoid: false,
				ReturnType: INamedTypeSymbol
				{
					TypeArguments: [{ } handleType]
				}
			}])
		{
			return true;
		}

		return SymbolEqualityComparer.IncludeNullability.Equals(handleType, paramType);
	}
}
