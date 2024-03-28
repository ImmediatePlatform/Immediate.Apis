using System.Collections.Immutable;
using System.Composition;
using Immediate.Apis.Analyzers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Simplification;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Immediate.Apis.CodeFixes;

/// <summary>
///     A sample code fix provider that renames classes with the company name in their definition.
///     All code fixes must  be linked to specific analyzers.
/// </summary>
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MissingHandlerAttributeCodeFixProvider))]
[Shared]
public class MissingHandlerAttributeCodeFixProvider : CodeFixProvider
{
	// Specify the diagnostic IDs of analyzers that are expected to be linked.
	public sealed override ImmutableArray<string> FixableDiagnosticIds { get; } =
		ImmutableArray.Create(DiagnosticIds.IAPI0001MissingHandlerAttribute);

	// If you don't need the 'fix all' behaviour, return null.
	public override FixAllProvider GetFixAllProvider()
	{
		return WellKnownFixAllProviders.BatchFixer;
	}

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
					title: "Add [Handler] attributes",
					createChangedDocument: c =>
						AddHandlerAttributeAsync(context.Document, compilationUnitSyntax, classDeclarationSyntax, c),
					equivalenceKey: nameof(MissingHandlerAttributeCodeFixProvider)
				),
				diagnostic);
		}
	}

	/// <summary>
	///     Executed on the quick fix action raised by the user.
	/// </summary>
	/// <param name="document">Affected source file.</param>
	/// <param name="classDeclarationSyntax">Highlighted class declaration Syntax Node.</param>
	/// <param name="cancellationToken">Any fix is cancellable by the user, so we should support the cancellation token.</param>
	/// <returns>Clone of the solution with updates: added handler attribute.</returns>
	private static async Task<Document> AddHandlerAttributeAsync(Document document,
		CompilationUnitSyntax root, ClassDeclarationSyntax classDeclarationSyntax, CancellationToken cancellationToken)
	{
		var model = await document.GetSemanticModelAsync(cancellationToken);

		var handlerAttributeSymbol =
			model?.Compilation.GetTypeByMetadataName("Immediate.Handlers.Shared.HandlerAttribute")!;

		var referenceId = DocumentationCommentId.CreateReferenceId(handlerAttributeSymbol);

		var annotation = new SyntaxAnnotation("SymbolId", referenceId);

		// Create the attribute syntax
		var handlerAttrSyntax = AttributeList(
				SingletonSeparatedList(
					Attribute(
						IdentifierName("Handler"))))
			.WithAdditionalAnnotations(Simplifier.AddImportsAnnotation, annotation);

		// Add the attribute to the class declaration
		var newClassDecl =
			classDeclarationSyntax.WithAttributeLists(
				classDeclarationSyntax.AttributeLists.Insert(0, handlerAttrSyntax));

		cancellationToken.ThrowIfCancellationRequested();

		// Replace the old class declaration with the new one
		var newRoot = root.ReplaceNode(classDeclarationSyntax, newClassDecl);

		cancellationToken.ThrowIfCancellationRequested();

		// Create a new document with the updated syntax root
		var newDocument = document.WithSyntaxRoot(newRoot);

		cancellationToken.ThrowIfCancellationRequested();

		return newDocument;
	}
}
