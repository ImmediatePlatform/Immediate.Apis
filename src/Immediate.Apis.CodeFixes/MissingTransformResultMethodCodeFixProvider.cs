using System.Collections.Immutable;
using Immediate.Apis.Analyzers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Immediate.Apis.CodeFixes;

[ExportCodeFixProvider(LanguageNames.CSharp)]
public class MissingTransformResultMethodCodeFixProvider : CodeFixProvider
{
	public sealed override ImmutableArray<string> FixableDiagnosticIds { get; } =
		ImmutableArray.Create([DiagnosticIds.IAPI0007MissingTransformResultMethod]);

	public override FixAllProvider? GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

	public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
	{
		// We link only one diagnostic and assume there is only one diagnostic in the context.
		var diagnostic = context.Diagnostics.Single();

		// 'SourceSpan' of 'Location' is the highlighted area. We're going to use this area to find the 'SyntaxNode' to rename.
		var diagnosticSpan = diagnostic.Location.SourceSpan;

		var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

		if (root?.FindNode(diagnosticSpan) is ClassDeclarationSyntax classDeclarationSyntax
			&& root is CompilationUnitSyntax compilationUnitSyntax)
		{
			context.RegisterCodeFix(
				CodeAction.Create(
					title: "Add `TransformResult` method",
					createChangedDocument: c =>
						AddTransformResultMethodAsync(context.Document, compilationUnitSyntax, classDeclarationSyntax, c),
					equivalenceKey: nameof(MissingCustomizeEndpointMethodCodeFixProvider)
				),
				diagnostic);
		}
	}

	private static async Task<Document> AddTransformResultMethodAsync(
		Document document,
		CompilationUnitSyntax root,
		ClassDeclarationSyntax classDeclarationSyntax,
		CancellationToken cancellationToken
	)
	{
		var model = await document.GetSemanticModelAsync(cancellationToken) ?? throw new InvalidOperationException("Could not get semantic model");

		if (model.GetDeclaredSymbol(classDeclarationSyntax, cancellationToken: cancellationToken) is not { } handlerClassSymbol)
			return document;

		var handleMethodSymbol = handlerClassSymbol
			.GetMembers()
			.OfType<IMethodSymbol>()
			.FirstOrDefault(x => x.Name is "Handle" or "HandleAsync");

		if (handleMethodSymbol is null)
			return document;

		if (await handleMethodSymbol.DeclaringSyntaxReferences[0]
				.GetSyntaxAsync(cancellationToken)
				is not MethodDeclarationSyntax handleMethodSyntax)
		{
			return document;
		}

		if (handleMethodSyntax.ReturnType is not GenericNameSyntax
			{
				TypeArgumentList.Arguments: [{ } returnType]
			})
		{
			return document;
		}

		var transformResultMethodSyntax = MethodDeclaration(
				returnType,
				Identifier("TransformResult"))
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
								Identifier("result"))
							.WithType(returnType))))
			.WithBody(
				Block(
					SingletonList<StatementSyntax>(
						ReturnStatement(
							IdentifierName("result")))))
			.WithAdditionalAnnotations(Formatter.Annotation);

		// Manually add trailing trivia to ensure proper spacing
		var newMembers = classDeclarationSyntax.Members
			.Insert(
				0,
				transformResultMethodSyntax
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
