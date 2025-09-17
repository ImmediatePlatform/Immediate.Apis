namespace Immediate.Apis.Tests.GeneratorTests;

public sealed class ParameterAttributeTests
{
	[Theory]
	[MemberData(nameof(Utility.Methods), MemberType = typeof(Utility))]
	public async Task IFormFileTest(string method)
	{
		var result = GeneratorTestHelper.RunGenerator(
			$$"""
			using System.Threading;
			using System.Threading.Tasks;
			using Microsoft.AspNetCore.Http;
			using Immediate.Apis.Shared;
			using Immediate.Handlers.Shared;
			
			namespace Dummy;

			[Handler]
			[Map{{method}}("/test")]
			public static partial class GetUsersQuery
			{
				public record Query
				{
					public required IFormFile File { get; init; }
				}

				private static ValueTask<int> Handle(
					Query _,
					CancellationToken token)
				{
					return ValueTask.FromResult(0);
				}
			}
			""");

		Assert.Equal(
			[
				@"Immediate.Apis.Generators/Immediate.Apis.Generators.ImmediateApisGenerator/RouteBuilder.Dummy_GetUsersQuery.g.cs",
				@"Immediate.Apis.Generators/Immediate.Apis.Generators.ImmediateApisGenerator/RoutesBuilder.g.cs",
				@"Immediate.Handlers.Generators/Immediate.Handlers.Generators.ImmediateHandlersGenerator/IH.Dummy.GetUsersQuery.g.cs",
				@"Immediate.Handlers.Generators/Immediate.Handlers.Generators.ImmediateHandlersGenerator/IH.ServiceCollectionExtensions.g.cs",
			],
			result.GeneratedTrees.Select(t => t.FilePath.Replace('\\', '/'))
		);

		_ = await Verify(result).UseParameters(method);
	}

	[Theory]
	[MemberData(nameof(Utility.Methods), MemberType = typeof(Utility))]
	public async Task AsParametersTest(string method)
	{
		var result = GeneratorTestHelper.RunGenerator(
			$$"""
			using System.Threading;
			using System.Threading.Tasks;
			using Microsoft.AspNetCore.Http;
			using Microsoft.AspNetCore.Mvc;
			using Immediate.Apis.Shared;
			using Immediate.Handlers.Shared;
			
			namespace Dummy;

			[Handler]
			[Map{{method}}("/test")]
			public static partial class GetUsersQuery
			{
				public record Query;

				private static ValueTask<int> Handle(
					[AsParameters]
					Query _,
					CancellationToken token)
				{
					return ValueTask.FromResult(0);
				}
			}
			""");

		Assert.Equal(
			[
				@"Immediate.Apis.Generators/Immediate.Apis.Generators.ImmediateApisGenerator/RouteBuilder.Dummy_GetUsersQuery.g.cs",
				@"Immediate.Apis.Generators/Immediate.Apis.Generators.ImmediateApisGenerator/RoutesBuilder.g.cs",
				@"Immediate.Handlers.Generators/Immediate.Handlers.Generators.ImmediateHandlersGenerator/IH.Dummy.GetUsersQuery.g.cs",
				@"Immediate.Handlers.Generators/Immediate.Handlers.Generators.ImmediateHandlersGenerator/IH.ServiceCollectionExtensions.g.cs",
			],
			result.GeneratedTrees.Select(t => t.FilePath.Replace('\\', '/'))
		);

		_ = await Verify(result).UseParameters(method);
	}

	private static TheoryData<string> FromXxx() =>
		[
			"FromBody",
			"FromForm",
			"FromHeader",
			"FromQuery",
			"FromRoute",
		];

	public static IEnumerable<(string, string)> GetMethodFromMatrix() =>
		Utility.Methods().SelectMany(_ => FromXxx(), (s1, s2) => (s1.Data, s2.Data));

	[Theory]
	[MemberData(nameof(GetMethodFromMatrix))]
	public async Task RequestParameterAttributeTest(string method, string source)
	{
		var result = GeneratorTestHelper.RunGenerator(
			$$"""
			using System.Threading;
			using System.Threading.Tasks;
			using Microsoft.AspNetCore.Http;
			using Microsoft.AspNetCore.Mvc;
			using Immediate.Apis.Shared;
			using Immediate.Handlers.Shared;
			
			namespace Dummy;

			[Handler]
			[Map{{method}}("/test")]
			public static partial class GetUsersQuery
			{
				public record Query;

				private static ValueTask<int> Handle(
					[{{source}}]
					Query _,
					CancellationToken token)
				{
					return ValueTask.FromResult(0);
				}
			}
			""");

		Assert.Equal(
			[
				@"Immediate.Apis.Generators/Immediate.Apis.Generators.ImmediateApisGenerator/RouteBuilder.Dummy_GetUsersQuery.g.cs",
				@"Immediate.Apis.Generators/Immediate.Apis.Generators.ImmediateApisGenerator/RoutesBuilder.g.cs",
				@"Immediate.Handlers.Generators/Immediate.Handlers.Generators.ImmediateHandlersGenerator/IH.Dummy.GetUsersQuery.g.cs",
				@"Immediate.Handlers.Generators/Immediate.Handlers.Generators.ImmediateHandlersGenerator/IH.ServiceCollectionExtensions.g.cs",
			],
			result.GeneratedTrees.Select(t => t.FilePath.Replace('\\', '/'))
		);

		_ = await Verify(result).UseParameters(method, source);
	}

	[Theory]
	[MemberData(nameof(GetMethodFromMatrix))]
	public async Task RequestPropertyAttributeTest(string method, string source)
	{
		var result = GeneratorTestHelper.RunGenerator(
			$$"""
			using System.Threading;
			using System.Threading.Tasks;
			using Microsoft.AspNetCore.Http;
			using Microsoft.AspNetCore.Mvc;
			using Immediate.Apis.Shared;
			using Immediate.Handlers.Shared;
			
			namespace Dummy;

			[Handler]
			[Map{{method}}("/test")]
			public static partial class GetUsersQuery
			{
				public record Query
				{
					[{{source}}]
					public required int Test { get; init; }
				}

				private static ValueTask<int> Handle(
					Query _,
					CancellationToken token)
				{
					return ValueTask.FromResult(0);
				}
			}
			""");

		Assert.Equal(
			[
				@"Immediate.Apis.Generators/Immediate.Apis.Generators.ImmediateApisGenerator/RouteBuilder.Dummy_GetUsersQuery.g.cs",
				@"Immediate.Apis.Generators/Immediate.Apis.Generators.ImmediateApisGenerator/RoutesBuilder.g.cs",
				@"Immediate.Handlers.Generators/Immediate.Handlers.Generators.ImmediateHandlersGenerator/IH.Dummy.GetUsersQuery.g.cs",
				@"Immediate.Handlers.Generators/Immediate.Handlers.Generators.ImmediateHandlersGenerator/IH.ServiceCollectionExtensions.g.cs",
			],
			result.GeneratedTrees.Select(t => t.FilePath.Replace('\\', '/'))
		);

		_ = await Verify(result).UseParameters(method, source);
	}
}
