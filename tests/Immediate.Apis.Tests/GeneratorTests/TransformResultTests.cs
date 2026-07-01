using static Immediate.Apis.Tests.Utility;

namespace Immediate.Apis.Tests.GeneratorTests;

public sealed class TransformResultTests
{
	[Theory]
	[MemberData(nameof(Methods), MemberType = typeof(Utility))]
	public async Task TransformWithReturnValid(string method)
	{
		var result = GeneratorTestHelper.RunGenerator(
			$$"""
			using System.Threading;
			using System.Threading.Tasks;
			using Immediate.Apis.Shared;
			using Immediate.Handlers.Shared;
			
			namespace Dummy;

			[Handler]
			[Map{{method}}("/test")]
			public static partial class GetUsersQuery
			{
				internal static double TransformResult(int x) => x;

				public record Query;

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
				@"Immediate.Apis.Generators/Immediate.Apis.Generators.ImmediateApisGenerator/IA.Dummy.GetUsersQuery.g.cs",
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
	public async Task TransformWithVoidValid(string method)
	{
		var result = GeneratorTestHelper.RunGenerator(
			$$"""
			using System.Threading;
			using System.Threading.Tasks;
			using Immediate.Apis.Shared;
			using Immediate.Handlers.Shared;
			
			namespace Dummy;

			[Handler]
			[Map{{method}}("/test")]
			public static partial class GetUsersQuery
			{
				internal static double TransformResult() => 1.0d;

				public record Query;

				private static ValueTask Handle(
					Query _,
					CancellationToken token)
				{
					return default;
				}
			}
			""");

		Assert.Equal(
			[
				@"Immediate.Apis.Generators/Immediate.Apis.Generators.ImmediateApisGenerator/IA.Dummy.GetUsersQuery.g.cs",
				@"Immediate.Apis.Generators/Immediate.Apis.Generators.ImmediateApisGenerator/IA.MapEndpoints.g.cs",
				@"Immediate.Handlers.Generators/Immediate.Handlers.Generators.ImmediateHandlersGenerator/IH.Dummy.GetUsersQuery.g.cs",
				@"Immediate.Handlers.Generators/Immediate.Handlers.Generators.ImmediateHandlersGenerator/IH.ServiceCollectionExtensions.g.cs",
			],
			result.GeneratedTrees.Select(t => t.FilePath.Replace('\\', '/'))
		);

		_ = await VerifyIgnoreImmediateHandlers(result).UseParameters(method);
	}
}
