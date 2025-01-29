using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Immediate.Apis.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class InvalidAuthorizeAttributeAnalyzer : DiagnosticAnalyzer
{
	public static readonly DiagnosticDescriptor InvalidAuthorizeParameter =
		new(
			id: DiagnosticIds.IAPI0002InvalidAuthorizeParameter,
			title: "Must use `Policies` parameter for [Authorize]",
			messageFormat: "[Authorize] was used with invalid parameter {0}",
			category: "ImmediateApis",
			defaultSeverity: DiagnosticSeverity.Error,
			isEnabledByDefault: true,
			description: "Only policy authorization is supported with Immediate.Apis."
		);

	public static readonly DiagnosticDescriptor UsedBothAuthorizeAndAnonymous =
		new(
			id: DiagnosticIds.IAPI0003UsedBothAuthorizeAndAnonymous,
			title: "Only use one of [AllowAnonymous] and [Authorize]",
			messageFormat: "Both [AllowAnonymous] and [Authorize] were used, but only one will be applied to the endpoint",
			category: "ImmediateApis",
			defaultSeverity: DiagnosticSeverity.Warning,
			isEnabledByDefault: true,
			description: "Only one of [AllowAnonymous] and [Authorize] can be used for a single endpoint."
		);

	public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
		ImmutableArray.Create(
		[
			InvalidAuthorizeParameter,
			UsedBothAuthorizeAndAnonymous,
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

		if (!attributes.Any(a => a.AttributeClass.IsMapMethodAttribute()))
			return;

		token.ThrowIfCancellationRequested();

		var allowAnonymous = attributes.Any(a => a.AttributeClass.IsAllowAnonymous());

		var authorizeAttribute = attributes.FirstOrDefault(a => a.AttributeClass.IsAuthorize());

		if (authorizeAttribute is not null)
		{
			if (allowAnonymous)
			{
				context.ReportDiagnostic(
					Diagnostic.Create(
						UsedBothAuthorizeAndAnonymous,
						namedTypeSymbol.Locations[0]
					)
				);
			}

			if (authorizeAttribute.NamedArguments.Length > 0)
			{
				foreach (var argument in authorizeAttribute.NamedArguments)
				{
					if (argument.Key is not "Policy")
					{
						context.ReportDiagnostic(
							Diagnostic.Create(
								InvalidAuthorizeParameter,
								authorizeAttribute.ApplicationSyntaxReference
									?.GetSyntax()
									.GetLocation(),
								argument.Key
							)
						);
					}
				}
			}
		}
	}
}
