using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Immediate.Apis.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class InvalidRoutesBuilderNameAnalyzer : DiagnosticAnalyzer
{
	public static readonly DiagnosticDescriptor InvalidRoutesBuilderName =
		new(
			id: DiagnosticIds.IAPI0011InvalidRoutesBuilderName,
			title: "Invalid routes builder name",
			messageFormat: "Routes builder name `{0}` is not a valid identifier",
			category: "ImmediateApis",
			defaultSeverity: DiagnosticSeverity.Error,
			isEnabledByDefault: true,
			description: "Routes builder names must be non-null, non-empty, and a valid C# identifier (start with a letter or underscore, followed by letters, digits, or underscores)."
		);

	public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
		ImmutableArray.Create(
		[
			InvalidRoutesBuilderName,
		]);

	public override void Initialize(AnalysisContext context)
	{
		if (context == null)
			throw new ArgumentNullException(nameof(context));

		context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
		context.EnableConcurrentExecution();

		context.RegisterSyntaxNodeAction(AnalyzeAttribute, SyntaxKind.Attribute);
	}

	private static void AnalyzeAttribute(SyntaxNodeAnalysisContext context)
	{
		var token = context.CancellationToken;
		token.ThrowIfCancellationRequested();

		if (context.Node is not AttributeSyntax
			{
				Parent: AttributeListSyntax { Target.Identifier.RawKind: (int)SyntaxKind.AssemblyKeyword },
				ArgumentList.Arguments: [{ Expression: var argumentExpression }, ..],
			} attribute)
		{
			return;
		}

		token.ThrowIfCancellationRequested();

		if (context.SemanticModel.GetSymbolInfo(attribute, token).Symbol?.ContainingType
			is not { } attributeType
			|| !attributeType.IsRoutesBuilderNameAttribute())
		{
			return;
		}

		token.ThrowIfCancellationRequested();

		var value = context.SemanticModel.GetConstantValue(argumentExpression, token) is { HasValue: true, Value: string s }
			? s
			: null;

		if (RoutesBuilderNameUtility.IsValidRoutesBuilderName(value))
			return;

		token.ThrowIfCancellationRequested();

		context.ReportDiagnostic(
			Diagnostic.Create(
				InvalidRoutesBuilderName,
				attribute.GetLocation(),
				value
			)
		);
	}
}
