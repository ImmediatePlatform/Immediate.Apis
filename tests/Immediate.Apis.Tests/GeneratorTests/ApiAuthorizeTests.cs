namespace Immediate.Apis.Tests.GeneratorTests;

public sealed class ApiAuthorizeTests
{
	[Theory]
	[InlineData("Get")]
	[InlineData("Post")]
	[InlineData("Patch")]
	[InlineData("Put")]
	[InlineData("Delete")]
	public async Task MapMethodWithSimpleAuthorizeTest(string method)
	{
		var driver = GeneratorTestHelper.GetDriver(
			$$"""
			using System.Threading.Tasks;
			using Immediate.Apis.Shared;
			using Immediate.Handlers.Shared;
			using Microsoft.AspNetCore.Authorization;
			
			namespace Dummy;

			[Handler]
			[Map{{method}}("/test")]
			[Authorize]
			public static class GetUsersQuery
			{
				public record Query;

				private static ValueTask<int> HandleAsync(
					Query _,
					CancellationToken token)
				{
					return 0;
				}
			}
			""");

		var result = driver.GetRunResult();

		Assert.Empty(result.Diagnostics);
		_ = Assert.Single(result.GeneratedTrees);

		_ = await Verify(result)
			.UseParameters(method);
	}

	[Theory]
	[InlineData("Get")]
	[InlineData("Post")]
	[InlineData("Patch")]
	[InlineData("Put")]
	[InlineData("Delete")]
	public async Task MapMethodWithAuthorizeConstructorTest(string method)
	{
		var driver = GeneratorTestHelper.GetDriver(
			$$"""
			using System.Threading.Tasks;
			using Immediate.Apis.Shared;
			using Immediate.Handlers.Shared;
			using Microsoft.AspNetCore.Authorization;
			
			namespace Dummy;

			[Handler]
			[Map{{method}}("/test")]
			[Authorize("TestPolicy")]
			public static class GetUsersQuery
			{
				public record Query;

				private static ValueTask<int> HandleAsync(
					Query _,
					CancellationToken token)
				{
					return 0;
				}
			}
			""");

		var result = driver.GetRunResult();

		Assert.Empty(result.Diagnostics);
		_ = Assert.Single(result.GeneratedTrees);

		_ = await Verify(result)
			.UseParameters(method);
	}

	[Theory]
	[InlineData("Get")]
	[InlineData("Post")]
	[InlineData("Patch")]
	[InlineData("Put")]
	[InlineData("Delete")]
	public async Task MapMethodWithAuthorizeNamedPolicyArgumentTest(string method)
	{
		var driver = GeneratorTestHelper.GetDriver(
			$$"""
			using System.Threading.Tasks;
			using Immediate.Apis.Shared;
			using Immediate.Handlers.Shared;
			using Microsoft.AspNetCore.Authorization;
			
			namespace Dummy;

			[Handler]
			[Map{{method}}("/test")]
			[Authorize(Policy = "TestPolicy")]
			public static class GetUsersQuery
			{
				public record Query;

				private static ValueTask<int> HandleAsync(
					Query _,
					CancellationToken token)
				{
					return 0;
				}
			}
			""");

		var result = driver.GetRunResult();

		Assert.Empty(result.Diagnostics);
		_ = Assert.Single(result.GeneratedTrees);

		_ = await Verify(result)
			.UseParameters(method);
	}
}
