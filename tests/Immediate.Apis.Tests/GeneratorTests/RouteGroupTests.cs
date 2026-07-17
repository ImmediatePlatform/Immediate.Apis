using static Immediate.Apis.Tests.Utility;

namespace Immediate.Apis.Tests.GeneratorTests;

public sealed class RouteGroupTests
{
	[Fact]
	public async Task SimpleRouteGroupTest()
	{
		var result = GeneratorTestHelper.RunGenerator(
			"""
			using System.Threading;
			using System.Threading.Tasks;
			using Immediate.Apis.Shared;
			using Immediate.Handlers.Shared;
			using Microsoft.AspNetCore.Routing;
			
			namespace Dummy;

			[RouteGroup("api/root")]
			public sealed partial class Root
			{
				[RouteGroup("mid")]
				public sealed partial class Mid
				{
					private static void CustomizeGroup(RouteGroupBuilder group)
					{
					}

					[RouteGroup("inner")]
					public sealed partial class Inner
					{
					}
				}
			}

			[Handler]
			[MapGet("test")]
			[MapGroup<Root.Mid.Inner>]
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
			"""
		);

		Assert.Equal(
			[
				@"Immediate.Apis.Generators/Immediate.Apis.Generators.ImmediateApisGenerator/IA.Dummy.GetUsersQuery.g.cs",
				@"Immediate.Apis.Generators/Immediate.Apis.Generators.ImmediateApisGenerator/IA.MapEndpoints.g.cs",
				@"Immediate.Apis.Generators/Immediate.Apis.Generators.ImmediateApisGenerator/IA.Dummy..Root.g.cs",
				@"Immediate.Handlers.Generators/Immediate.Handlers.Generators.ImmediateHandlersGenerator/IH.Dummy.GetUsersQuery.g.cs",
				@"Immediate.Handlers.Generators/Immediate.Handlers.Generators.ImmediateHandlersGenerator/IH.ServiceCollectionExtensions.g.cs",
			],
			result.GeneratedTrees.Select(t => t.FilePath.Replace('\\', '/'))
		);

		_ = await VerifyIgnoreImmediateHandlers(result);
	}

	[Theory]
	[MemberData(nameof(Frameworks))]
	public async Task TaggedRouteGroupTest(string framework)
	{
		var result = GeneratorTestHelper.RunGenerator(
			"""
			using System.Threading;
			using System.Threading.Tasks;
			using Immediate.Apis.Shared;
			using Immediate.Handlers.Shared;
			using Microsoft.AspNetCore.Routing;
			
			namespace Dummy;

			[RouteGroup("api/root", Tags = ["Root"])]
			public sealed partial class Root
			{
				[RouteGroup("mid", Tags = ["Middle"])]
				public sealed partial class Mid
				{
					private static void CustomizeGroup(RouteGroupBuilder group)
					{
					}

					[RouteGroup("inner")]
					public sealed partial class Inner
					{
					}
				}
			}
			
			[Handler]
			[MapGet("test")]
			[MapGroup<Root.Mid.Inner>]
			public static partial class GetUsersQuery1
			{
				public record Query;
			
				private static ValueTask<int> Handle(
					Query _,
					CancellationToken token)
				{
					return ValueTask.FromResult(0);
				}
			}
			
			[Handler]
			[MapGet("test2", Tags = ["Class"])]
			[MapGroup<Root.Mid.Inner>]
			public static partial class GetUsersQuery2
			{
				public record Query;
			
				private static ValueTask<int> Handle(
					Query _,
					CancellationToken token)
				{
					return ValueTask.FromResult(0);
				}
			}
			"""
		);

		Assert.Equal(
			[
				@"Immediate.Apis.Generators/Immediate.Apis.Generators.ImmediateApisGenerator/IA.Dummy.GetUsersQuery1.g.cs",
				@"Immediate.Apis.Generators/Immediate.Apis.Generators.ImmediateApisGenerator/IA.Dummy.GetUsersQuery2.g.cs",
				@"Immediate.Apis.Generators/Immediate.Apis.Generators.ImmediateApisGenerator/IA.MapEndpoints.g.cs",
				@"Immediate.Apis.Generators/Immediate.Apis.Generators.ImmediateApisGenerator/IA.Dummy..Root.g.cs",
				@"Immediate.Handlers.Generators/Immediate.Handlers.Generators.ImmediateHandlersGenerator/IH.Dummy.GetUsersQuery1.g.cs",
				@"Immediate.Handlers.Generators/Immediate.Handlers.Generators.ImmediateHandlersGenerator/IH.Dummy.GetUsersQuery2.g.cs",
				@"Immediate.Handlers.Generators/Immediate.Handlers.Generators.ImmediateHandlersGenerator/IH.ServiceCollectionExtensions.g.cs",
			],
			result.GeneratedTrees.Select(t => t.FilePath.Replace('\\', '/'))
		);

		_ = await Verify(result)
			.UseParameters(framework);
	}

	public static TheoryData<string> Frameworks =>
		[ReferenceAssemblies.TargetFramework];
}
