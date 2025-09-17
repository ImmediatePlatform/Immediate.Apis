using System.Collections.Immutable;
using Immediate.Apis.Analyzers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;
using Microsoft.CodeAnalysis.Simplification;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Immediate.Apis.CodeFixes;

[ExportCodeFixProvider(LanguageNames.CSharp)]
public sealed class MissingCustomizeEndpointMethodCodeFixProvider : CodeFixProvider
{
	public sealed override ImmutableArray<string> FixableDiagnosticIds { get; } =
		ImmutableArray.Create([DiagnosticIds.IAPI0006MissingCustomizeEndpointMethod]);

	public override FixAllProvider? GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

	public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
	{
		// We link only one diagnostic and assume there is only one diagnostic in the context.
		var diagnostic = context.Diagnostics.Single();

		// 'SourceSpan' of 'Location' is the highlighted area. We're going to use this area to find the 'SyntaxNode' to rename.
		var diagnosticSpan = diagnostic.Location.SourceSpan;

		var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

		if (root?.FindNode(diagnosticSpan) is ClassDeclarationSyntax classDeclarationSyntax &&
			root is CompilationUnitSyntax compilationUnitSyntax)
		{
			context.RegisterCodeFix(
				CodeAction.Create(
					title: "Add `CustomizeEndpoint` method",
					createChangedDocument: c =>
						AddCustomizeEndpointMethodAsync(context.Document, compilationUnitSyntax, classDeclarationSyntax, c),
					equivalenceKey: nameof(MissingCustomizeEndpointMethodCodeFixProvider)
				),
				diagnostic);
		}
	}

	private static async Task<Document> AddCustomizeEndpointMethodAsync(
		Document document,
		CompilationUnitSyntax root,
		ClassDeclarationSyntax classDeclarationSyntax,
		CancellationToken cancellationToken
	)
	{
		var model = await document.GetSemanticModelAsync(cancellationToken);

		var endpointConventionBuilderSymbol = model?.Compilation
			.GetTypeByMetadataName("Microsoft.AspNetCore.Builder.RouteHandlerBuilder")!;

		var referenceId = DocumentationCommentId.CreateReferenceId(endpointConventionBuilderSymbol);
		var annotation = new SyntaxAnnotation("SymbolId", referenceId);

		var customizeEndpointMethodSyntax = MethodDeclaration(
				PredefinedType(
					Token(SyntaxKind.VoidKeyword)),
				Identifier("CustomizeEndpoint"))
			.WithModifiers(
				TokenList(
				[
					Token(SyntaxKind.InternalKeyword),
					Token(SyntaxKind.StaticKeyword),
				]))
			.WithParameterList(
				ParameterList(
					SingletonSeparatedList(
						Parameter(
								Identifier("endpoint"))
							.WithType(
								IdentifierName("RouteHandlerBuilder")))))
			.WithExpressionBody(
				ArrowExpressionClause(
					IdentifierName("endpoint")))
			.WithSemicolonToken(
				Token(SyntaxKind.SemicolonToken))
			.WithAdditionalAnnotations(Simplifier.AddImportsAnnotation, annotation)
			.WithAdditionalAnnotations(Formatter.Annotation);

		// Manually add trailing trivia to ensure proper spacing
		var newMembers = classDeclarationSyntax.Members
			.Insert(
				0,
				customizeEndpointMethodSyntax
			);

		var newClassDecl = classDeclarationSyntax
			.WithMembers(newMembers);

		// Replace the old class declaration with the new one
		var newRoot = root.ReplaceNode(classDeclarationSyntax, newClassDecl);

		// Create a new document with the updated syntax root
		var newDocument = document.WithSyntaxRoot(newRoot);

		// Return the new document
		return newDocument;
	}
}
