using static Immediate.Apis.Tests.Utility;

namespace Immediate.Apis.Tests.GeneratorTests;

public sealed class InvalidCodeTests
{
	[Theory]
	[MemberData(nameof(Methods), MemberType = typeof(Utility))]
	public async Task MissingHandlerAttribute(string method)
	{
		var result = GeneratorTestHelper.RunGenerator(
			$$"""
			using System.Threading;
			using System.Threading.Tasks;
			using Immediate.Apis.Shared;
			
			namespace Dummy;

			[Map{{method}}("/test")]
			public static partial class GetUsersQuery
			{
				public record Query;

				private static ValueTask<int> Handle(
					Query _,
					CancellationToken token)
				{
					return ValueTask.FromResult(0);
				}
			}
			""",
			skippedSteps: ["Endpoints"]
		);

		Assert.Equal(
			[
				@"Immediate.Apis.Generators/Immediate.Apis.Generators.ImmediateApisGenerator/IA.MapEndpoints.g.cs",
			],
			result.GeneratedTrees.Select(t => t.FilePath.Replace('\\', '/'))
		);

		_ = await VerifyIgnoreImmediateHandlers(result).UseParameters(method);
	}

	[Theory]
	[MemberData(nameof(Methods), MemberType = typeof(Utility))]
	public async Task MissingHandler(string method)
	{
		var result = GeneratorTestHelper.RunGenerator(
			$$"""
			using System.Threading.Tasks;
			using Immediate.Apis.Shared;
			using Immediate.Handlers.Shared;
			
			namespace Dummy;
			
			[Handler]
			[Map{{method}}("/test")]
			public static partial class GetUsersQuery
			{
				public record Query;
			}
			""",
			skippedSteps: ["Endpoints"]
		);

		Assert.Equal(
			[
				@"Immediate.Apis.Generators/Immediate.Apis.Generators.ImmediateApisGenerator/IA.MapEndpoints.g.cs",
			],
			result.GeneratedTrees.Select(t => t.FilePath.Replace('\\', '/'))
		);

		_ = await VerifyIgnoreImmediateHandlers(result).UseParameters(method);
	}

	[Theory]
	[MemberData(nameof(Methods), MemberType = typeof(Utility))]
	public async Task InvalidHandler(string method)
	{
		var result = GeneratorTestHelper.RunGenerator(
			$$"""
			using System.Threading.Tasks;
			using Immediate.Apis.Shared;
			using Immediate.Handlers.Shared;
			
			namespace Dummy;
			
			[Handler]
			[Map{{method}}("/test")]
			public static partial class GetUsersQuery
			{
				public record Query;

				private static ValueTask<int> Handle()
				{
					return ValueTask.FromResult(0);
				}
			}
			""",
			skippedSteps: ["Endpoints"]
		);

		Assert.Equal(
			[
				@"Immediate.Apis.Generators/Immediate.Apis.Generators.ImmediateApisGenerator/IA.MapEndpoints.g.cs",
			],
			result.GeneratedTrees.Select(t => t.FilePath.Replace('\\', '/'))
		);

		_ = await VerifyIgnoreImmediateHandlers(result).UseParameters(method);
	}

	[Theory]
	[MemberData(nameof(Methods), MemberType = typeof(Utility))]
	public async Task AuthorizeUsesAuthenticationSchemes(string method)
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
			[Authorize(AuthenticationSchemes = "test")]
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
			""",
			skippedSteps: ["Endpoints"]
		);

		Assert.Equal(
			[
				@"Immediate.Apis.Generators/Immediate.Apis.Generators.ImmediateApisGenerator/IA.MapEndpoints.g.cs",
				@"Immediate.Handlers.Generators/Immediate.Handlers.Generators.ImmediateHandlersGenerator/IH.Dummy.GetUsersQuery.g.cs",
				@"Immediate.Handlers.Generators/Immediate.Handlers.Generators.ImmediateHandlersGenerator/IH.ServiceCollectionExtensions.g.cs",
			],
			result.GeneratedTrees.Select(t => t.FilePath.Replace('\\', '/'))
		);

		_ = await VerifyIgnoreImmediateHandlers(result).UseParameters(method);
	}

	[Theory]
	[MemberData(nameof(Methods), MemberType = typeof(Utility))]
	public async Task AuthorizeUsesRoles(string method)
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
			[Authorize(Roles = "test")]
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
			""",
			skippedSteps: ["Endpoints"]
		);

		Assert.Equal(
			[
				@"Immediate.Apis.Generators/Immediate.Apis.Generators.ImmediateApisGenerator/IA.MapEndpoints.g.cs",
				@"Immediate.Handlers.Generators/Immediate.Handlers.Generators.ImmediateHandlersGenerator/IH.Dummy.GetUsersQuery.g.cs",
				@"Immediate.Handlers.Generators/Immediate.Handlers.Generators.ImmediateHandlersGenerator/IH.ServiceCollectionExtensions.g.cs",
			],
			result.GeneratedTrees.Select(t => t.FilePath.Replace('\\', '/'))
		);

		_ = await VerifyIgnoreImmediateHandlers(result).UseParameters(method);
	}
}
