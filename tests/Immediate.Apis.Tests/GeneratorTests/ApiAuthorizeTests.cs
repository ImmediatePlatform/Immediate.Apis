namespace Immediate.Apis.Tests.GeneratorTests;

public sealed class ApiAuthorizeTests
{
	[Theory]
	[MemberData(nameof(Utility.Methods), MemberType = typeof(Utility))]
	public async Task MapMethodWithSimpleAuthorizeTest(string method)
	{
		var result = GeneratorTestHelper.RunGenerator(
			$$"""
			using System.Threading;
			using System.Threading.Tasks;
			using Immediate.Apis.Shared;
			using Immediate.Handlers.Shared;
			using Microsoft.AspNetCore.Authorization;
			
			namespace Dummy;

			[Handler]
			[Map{{method}}("/test")]
			[Authorize]
			public static partial class GetUsersQuery
			{
				public record Query;

				private static ValueTask<int> HandleAsync(
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
				@"Immediate.Handlers.Generators/Immediate.Handlers.Generators.ImmediateHandlers.ImmediateHandlersGenerator/IH.Dummy.GetUsersQuery.g.cs",
				@"Immediate.Handlers.Generators/Immediate.Handlers.Generators.ImmediateHandlers.ImmediateHandlersGenerator/IH.ServiceCollectionExtensions.g.cs",
			],
			result.GeneratedTrees.Select(t => t.FilePath.Replace('\\', '/'))
		);

		_ = await Verify(result).UseParameters(method);
	}

	[Theory]
	[MemberData(nameof(Utility.Methods), MemberType = typeof(Utility))]
	public async Task MapMethodWithAuthorizeConstructorTest(string method)
	{
		var result = GeneratorTestHelper.RunGenerator(
			$$"""
			using System.Threading;
			using System.Threading.Tasks;
			using Immediate.Apis.Shared;
			using Immediate.Handlers.Shared;
			using Microsoft.AspNetCore.Authorization;
			
			namespace Dummy;

			[Handler]
			[Map{{method}}("/test")]
			[Authorize("TestPolicy")]
			public static partial class GetUsersQuery
			{
				public record Query;

				private static ValueTask<int> HandleAsync(
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
				@"Immediate.Handlers.Generators/Immediate.Handlers.Generators.ImmediateHandlers.ImmediateHandlersGenerator/IH.Dummy.GetUsersQuery.g.cs",
				@"Immediate.Handlers.Generators/Immediate.Handlers.Generators.ImmediateHandlers.ImmediateHandlersGenerator/IH.ServiceCollectionExtensions.g.cs",
			],
			result.GeneratedTrees.Select(t => t.FilePath.Replace('\\', '/'))
		);

		_ = await Verify(result).UseParameters(method);
	}

	[Theory]
	[MemberData(nameof(Utility.Methods), MemberType = typeof(Utility))]
	public async Task MapMethodWithAuthorizeNamedPolicyArgumentTest(string method)
	{
		var result = GeneratorTestHelper.RunGenerator(
			$$"""
			using System.Threading;
			using System.Threading.Tasks;
			using Immediate.Apis.Shared;
			using Immediate.Handlers.Shared;
			using Microsoft.AspNetCore.Authorization;
			
			namespace Dummy;

			[Handler]
			[Map{{method}}("/test")]
			[Authorize(Policy = "TestPolicy")]
			public static partial class GetUsersQuery
			{
				public record Query;

				private static ValueTask<int> HandleAsync(
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
				@"Immediate.Handlers.Generators/Immediate.Handlers.Generators.ImmediateHandlers.ImmediateHandlersGenerator/IH.Dummy.GetUsersQuery.g.cs",
				@"Immediate.Handlers.Generators/Immediate.Handlers.Generators.ImmediateHandlers.ImmediateHandlersGenerator/IH.ServiceCollectionExtensions.g.cs",
			],
			result.GeneratedTrees.Select(t => t.FilePath.Replace('\\', '/'))
		);

		_ = await Verify(result).UseParameters(method);
	}
}
