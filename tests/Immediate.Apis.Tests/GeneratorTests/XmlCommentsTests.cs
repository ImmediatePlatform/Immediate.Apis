using static Immediate.Apis.Tests.Utility;

namespace Immediate.Apis.Tests.GeneratorTests;

public sealed class XmlCommentsTests
{
	[Fact]
	public async Task MapsHandlerAndRequestCommentsToEndpointMetadata()
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

		_ = await VerifyIgnoreImmediateHandlers(result);
	}

	[Fact]
	public async Task UsesBodyParameterCommentAndPreservesExplicitSummary()
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

		_ = await VerifyIgnoreImmediateHandlers(result);
	}
}
