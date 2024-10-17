namespace Immediate.Apis.Tests.GeneratorTests;

public sealed class InvalidCodeTests
{
	[Test]
	[MethodDataSource(typeof(Utility), nameof(Utility.Methods))]
	public async Task MissingHandlerAttribute(string method)
	{
		var result = await GeneratorTestHelper.RunGenerator(
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
			""");

		_ = await Assert
			.That(result.GeneratedTrees.Select(t => t.FilePath.Replace('\\', '/')))
			.IsEquivalentCollectionTo([
				@"Immediate.Handlers.Generators/Immediate.Handlers.Generators.ImmediateHandlers.ImmediateHandlersGenerator/IH.ServiceCollectionExtensions.g.cs",
			]);

		_ = await Verify(result).UseParameters(method);
	}

	[Test]
	[MethodDataSource(typeof(Utility), nameof(Utility.Methods))]
	public async Task MissingHandler(string method)
	{
		var result = await GeneratorTestHelper.RunGenerator(
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
			""");

		_ = await Assert
			.That(result.GeneratedTrees.Select(t => t.FilePath.Replace('\\', '/')))
			.IsEmpty();

		_ = await Verify(result).UseParameters(method);
	}

	[Test]
	[MethodDataSource(typeof(Utility), nameof(Utility.Methods))]
	public async Task InvalidHandler(string method)
	{
		var result = await GeneratorTestHelper.RunGenerator(
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
			""");

		_ = await Assert
			.That(result.GeneratedTrees.Select(t => t.FilePath.Replace('\\', '/')))
			.IsEmpty();

		_ = await Verify(result).UseParameters(method);
	}

	[Test]
	[MethodDataSource(typeof(Utility), nameof(Utility.Methods))]
	public async Task AuthorizeUsesAuthenticationSchemes(string method)
	{
		var result = await GeneratorTestHelper.RunGenerator(
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
			""");

		_ = await Assert
			.That(result.GeneratedTrees.Select(t => t.FilePath.Replace('\\', '/')))
			.IsEquivalentCollectionTo([
				@"Immediate.Handlers.Generators/Immediate.Handlers.Generators.ImmediateHandlers.ImmediateHandlersGenerator/IH.Dummy.GetUsersQuery.g.cs",
				@"Immediate.Handlers.Generators/Immediate.Handlers.Generators.ImmediateHandlers.ImmediateHandlersGenerator/IH.ServiceCollectionExtensions.g.cs",
			]);

		_ = await Verify(result).UseParameters(method);
	}

	[Test]
	[MethodDataSource(typeof(Utility), nameof(Utility.Methods))]
	public async Task AuthorizeUsesRoles(string method)
	{
		var result = await GeneratorTestHelper.RunGenerator(
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
			""");

		_ = await Assert
			.That(result.GeneratedTrees.Select(t => t.FilePath.Replace('\\', '/')))
			.IsEquivalentCollectionTo([
				@"Immediate.Handlers.Generators/Immediate.Handlers.Generators.ImmediateHandlers.ImmediateHandlersGenerator/IH.Dummy.GetUsersQuery.g.cs",
				@"Immediate.Handlers.Generators/Immediate.Handlers.Generators.ImmediateHandlers.ImmediateHandlersGenerator/IH.ServiceCollectionExtensions.g.cs",
			]);

		_ = await Verify(result).UseParameters(method);
	}
}
