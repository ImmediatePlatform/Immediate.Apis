using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Immediate.Apis.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class EndpointAsDependencyAnalyzer : DiagnosticAnalyzer
{
	public static readonly DiagnosticDescriptor HandlerShouldNotDependOnEndpoint =
		new(
			id: DiagnosticIds.IAPI0008HandlerShouldNotDependOnEndpoint,
			title: "Handler should not depend on an endpoint handler",
			messageFormat: "Type `{0}` is an endpoint handler and should not be consumed by another handler",
			category: "ImmediateApis",
			defaultSeverity: DiagnosticSeverity.Info,
			isEnabledByDefault: true,
			description: "An endpoint should only be used externally; internal use is usually a sign of poor design."
		);

	public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
		ImmutableArray.Create(
		[
			HandlerShouldNotDependOnEndpoint,
		]);

	public override void Initialize(AnalysisContext context)
	{
		if (context == null)
			throw new ArgumentNullException(nameof(context));

		context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
		context.EnableConcurrentExecution();

		context.RegisterSymbolAction(AnalyzeSymbol, SymbolKind.Method);
	}

	private void AnalyzeSymbol(SymbolAnalysisContext context)
	{
		var token = context.CancellationToken;
		token.ThrowIfCancellationRequested();

		if (context.Symbol is not IMethodSymbol { Parameters: { } parameters })
			return;

		foreach (var parameter in parameters)
		{
			token.ThrowIfCancellationRequested();

			if (parameter.Type is not
				{
					Name: "Handler",
					IsStatic: false,
					ContainingType:
					{
						IsStatic: true,
					} parameterHandlerContainingType,
				})
			{
				continue;
			}

			if (!parameterHandlerContainingType.GetAttributes().Any(a => a.AttributeClass.IsHandlerAttribute()))
				continue;

			if (!parameterHandlerContainingType.GetAttributes().Any(a => a.AttributeClass.IsMapMethodAttribute()))
				continue;

			var parameterSyntax = (ParameterSyntax)parameter.DeclaringSyntaxReferences.First().GetSyntax(token);
			var parameterTypeSyntax = parameterSyntax.Type!;

			context.ReportDiagnostic(
				Diagnostic.Create(
					HandlerShouldNotDependOnEndpoint,
					parameterTypeSyntax.GetLocation(),
					parameterHandlerContainingType.Name
				)
			);
		}
	}
}
