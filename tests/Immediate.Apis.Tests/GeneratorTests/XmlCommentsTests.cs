using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Immediate.Apis.Tests.GeneratorTests;

public sealed class XmlCommentsTests
{
	[Fact]
	public void MapsHandlerAndRequestCommentsToEndpointMetadata()
	{
		var result = GeneratorTestHelper.RunGenerator(
			"""
			#nullable enable

			using System.Threading.Tasks;
			using Immediate.Apis.Shared;
			using Immediate.Handlers.Shared;
			using Microsoft.AspNetCore.Mvc;

			/// <summary>Find a <see cref="Todo"/>.</summary>
			/// <remarks>Reads from the &quot;active&quot; list.</remarks>
			[Handler]
			[MapGet("/todos/{todo_id}")]
			public static partial class GetTodo
			{
				/// <param name="Id">The todo identifier.</param>
				public sealed record Query([FromRoute(Name = "todo_id")] int Id)
				{
					/// <summary>A search <c>filter</c>.</summary>
					public string? Filter { get; init; }
				}

				public sealed record Todo(int Id);

				/// <returns>The matching item.</returns>
				/// <response code="404">No todo matches.</response>
				private static ValueTask<Todo> Handle(Query query) => ValueTask.FromResult(new Todo(query.Id));
			}
			""");

		var source = result.Results[0].GeneratedSources.Single(s => s.HintName.EndsWith("GetTodo.g.cs", StringComparison.Ordinal)).SourceText.ToString();
		Assert.Contains("WithSummary(endpoint, \"Find a Todo.\")", source, StringComparison.Ordinal);
		Assert.Contains("WithDescription(endpoint, \"Reads from the \\\"active\\\" list.\")", source, StringComparison.Ordinal);
		Assert.Contains("[\"Id\"] = \"The todo identifier.\"", source, StringComparison.Ordinal);
		Assert.Contains("[\"Filter\"] = \"A search `filter`.\"", source, StringComparison.Ordinal);
		Assert.Contains("\"The matching item.\"", source, StringComparison.Ordinal);
		Assert.Contains("[\"404\"] = \"No todo matches.\"", source, StringComparison.Ordinal);
		Assert.DoesNotContain(result.Results[0].GeneratedSources, s => s.HintName is "IA.XmlComments.g.cs");
	}

	[Fact]
	public void UsesBodyParameterCommentAndPreservesExplicitSummary()
	{
		var result = GeneratorTestHelper.RunGenerator(
			"""
			using System.Threading.Tasks;
			using Immediate.Apis.Shared;
			using Immediate.Handlers.Shared;
			using Microsoft.AspNetCore.Http;

			[Handler]
			[MapPost("/todos", "/archived-todos")]
			public static partial class AddTodo
			{
				/// <summary>Fallback body description.</summary>
				public sealed record Command(string Name);

				/// <summary>Fallback summary.</summary>
				/// <remarks>Creates a new todo in the requested list.</remarks>
				/// <param name="command">The todo to create.</param>
				[EndpointSummary("Configured summary.")]
				private static ValueTask<string> Handle(Command command) => ValueTask.FromResult(command.Name);
			}
			""");

		var route = Assert.Single(result.Results[0].GeneratedSources, s => s.HintName.EndsWith("AddTodo.g.cs", StringComparison.Ordinal));
		var source = route.SourceText.ToString();
		Assert.Contains("EndpointSummaryAttribute(\"Configured summary.\")", source, StringComparison.Ordinal);
		Assert.DoesNotContain("WithSummary(", source, StringComparison.Ordinal);
		Assert.Contains("WithDescription(endpoint, \"Creates a new todo in the requested list.\")", source, StringComparison.Ordinal);

		var bodyDescriptions = route.SyntaxTree.GetRoot(TestContext.Current.CancellationToken)
			.DescendantNodes()
			.OfType<ObjectCreationExpressionSyntax>()
			.Where(node => node.Type.ToString() is "global::Immediate.Apis.Shared.EndpointXmlCommentsMetadata")
			.Select(node => node.ArgumentList!.Arguments[1].Expression.ToString());
		Assert.Equal(["\"The todo to create.\"", "\"The todo to create.\""], bodyDescriptions);
	}
}
