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
	}
}
