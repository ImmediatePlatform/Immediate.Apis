namespace Immediate.Apis.Tests.GeneratorTests;

public sealed class ParameterAttributeTests
{
	[Theory]
	[MemberData(nameof(Utility.Methods), MemberType = typeof(Utility))]
	public async Task IFormFileTest(string method)
	{
		var driver = GeneratorTestHelper.GetDriver(
			$$"""
			using System.Threading.Tasks;
			using Microsoft.AspNetCore.Http;
			using Immediate.Apis.Shared;
			using Immediate.Handlers.Shared;
			
			namespace Dummy;

			[Handler]
			[Map{{method}}("/test")]
			public static class GetUsersQuery
			{
				public record Query
				{
					public required IFormFile File { get; init; }
				}

				private static ValueTask<int> Handle(
					Query _,
					CancellationToken token)
				{
					return 0;
				}
			}
			""");

		var result = driver.GetRunResult();

		Assert.Empty(result.Diagnostics);
		Assert.Equal(2, result.GeneratedTrees.Length);

		_ = await Verify(result)
			.UseParameters(method);
	}

	[Theory]
	[MemberData(nameof(Utility.Methods), MemberType = typeof(Utility))]
	public async Task AsParametersTest(string method)
	{
		var driver = GeneratorTestHelper.GetDriver(
			$$"""
			using System.Threading.Tasks;
			using Immediate.Apis.Shared;
			using Immediate.Handlers.Shared;
			
			namespace Dummy;

			[Handler]
			[Map{{method}}("/test")]
			public static class GetUsersQuery
			{
				[EndpointRegistrationOverride(EndpointRegistration.AsParameters)]
				public record Query;

				private static ValueTask<int> Handle(
					Query _,
					CancellationToken token)
				{
					return 0;
				}
			}
			""");

		var result = driver.GetRunResult();

		Assert.Empty(result.Diagnostics);
		Assert.Equal(2, result.GeneratedTrees.Length);

		_ = await Verify(result)
			.UseParameters(method);
	}

	[Theory]
	[MemberData(nameof(Utility.Methods), MemberType = typeof(Utility))]
	public async Task FromBodyTest(string method)
	{
		var driver = GeneratorTestHelper.GetDriver(
			$$"""
			using System.Threading.Tasks;
			using Immediate.Apis.Shared;
			using Immediate.Handlers.Shared;
			
			namespace Dummy;

			[Handler]
			[Map{{method}}("/test")]
			public static class GetUsersQuery
			{
				[EndpointRegistrationOverride(EndpointRegistration.FromBody)]
				public record Query;

				private static ValueTask<int> Handle(
					Query _,
					CancellationToken token)
				{
					return 0;
				}
			}
			""");

		var result = driver.GetRunResult();

		Assert.Empty(result.Diagnostics);
		Assert.Equal(2, result.GeneratedTrees.Length);

		_ = await Verify(result)
			.UseParameters(method);
	}

	[Theory]
	[MemberData(nameof(Utility.Methods), MemberType = typeof(Utility))]
	public async Task FromFormTest(string method)
	{
		var driver = GeneratorTestHelper.GetDriver(
			$$"""
			using System.Threading.Tasks;
			using Immediate.Apis.Shared;
			using Immediate.Handlers.Shared;
			
			namespace Dummy;

			[Handler]
			[Map{{method}}("/test")]
			public static class GetUsersQuery
			{
				[EndpointRegistrationOverride(EndpointRegistration.FromForm)]
				public record Query;

				private static ValueTask<int> Handle(
					Query _,
					CancellationToken token)
				{
					return 0;
				}
			}
			""");

		var result = driver.GetRunResult();

		Assert.Empty(result.Diagnostics);
		Assert.Equal(2, result.GeneratedTrees.Length);

		_ = await Verify(result)
			.UseParameters(method);
	}

	[Theory]
	[MemberData(nameof(Utility.Methods), MemberType = typeof(Utility))]
	public async Task FromHeaderTest(string method)
	{
		var driver = GeneratorTestHelper.GetDriver(
			$$"""
			using System.Threading.Tasks;
			using Immediate.Apis.Shared;
			using Immediate.Handlers.Shared;
			
			namespace Dummy;

			[Handler]
			[Map{{method}}("/test")]
			public static class GetUsersQuery
			{
				[EndpointRegistrationOverride(EndpointRegistration.FromHeaders)]
				public record Query;

				private static ValueTask<int> Handle(
					Query _,
					CancellationToken token)
				{
					return 0;
				}
			}
			""");

		var result = driver.GetRunResult();

		Assert.Empty(result.Diagnostics);
		Assert.Equal(2, result.GeneratedTrees.Length);

		_ = await Verify(result)
			.UseParameters(method);
	}

	[Theory]
	[MemberData(nameof(Utility.Methods), MemberType = typeof(Utility))]
	public async Task FromQueryTest(string method)
	{
		var driver = GeneratorTestHelper.GetDriver(
			$$"""
			using System.Threading.Tasks;
			using Immediate.Apis.Shared;
			using Immediate.Handlers.Shared;
			
			namespace Dummy;

			[Handler]
			[Map{{method}}("/test")]
			public static class GetUsersQuery
			{
				[EndpointRegistrationOverride(EndpointRegistration.FromQuery)]
				public record Query;

				private static ValueTask<int> Handle(
					Query _,
					CancellationToken token)
				{
					return 0;
				}
			}
			""");

		var result = driver.GetRunResult();

		Assert.Empty(result.Diagnostics);
		Assert.Equal(2, result.GeneratedTrees.Length);

		_ = await Verify(result)
			.UseParameters(method);
	}

	[Theory]
	[MemberData(nameof(Utility.Methods), MemberType = typeof(Utility))]
	public async Task FromRouteTest(string method)
	{
		var driver = GeneratorTestHelper.GetDriver(
			$$"""
			using System.Threading.Tasks;
			using Immediate.Apis.Shared;
			using Immediate.Handlers.Shared;
			
			namespace Dummy;

			[Handler]
			[Map{{method}}("/test")]
			public static class GetUsersQuery
			{
				[EndpointRegistrationOverride(EndpointRegistration.FromRoute)]
				public record Query;

				private static ValueTask<int> Handle(
					Query _,
					CancellationToken token)
				{
					return 0;
				}
			}
			""");

		var result = driver.GetRunResult();

		Assert.Empty(result.Diagnostics);
		Assert.Equal(2, result.GeneratedTrees.Length);

		_ = await Verify(result)
			.UseParameters(method);
	}
}
