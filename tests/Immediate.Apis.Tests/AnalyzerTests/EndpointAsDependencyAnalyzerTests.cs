using Immediate.Apis.Analyzers;
using Immediate.Handlers.Generators.ImmediateHandlers;

namespace Immediate.Apis.Tests.AnalyzerTests;

public sealed class EndpointAsDependencyAnalyzerTests
{
	[Test]
	[MethodDataSource(typeof(Utility), nameof(Utility.Methods))]
	public async Task HandlerMethodDependsOnEndpointHandlerShouldWarn(string method) =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<EndpointAsDependencyAnalyzer, ImmediateHandlersGenerator>(
			$$"""
			using System.Threading;
			using System.Threading.Tasks;
			using Immediate.Apis.Shared;
			using Immediate.Handlers.Shared;
			using Microsoft.AspNetCore.Authorization;
			using Microsoft.AspNetCore.Http;
					
			namespace Dummy;

			[Handler]
			[Map{{method}}("/test")]
			[Authorize(Roles = "")]
			public static partial class GetUsersQuery
			{
				public record Query;

				private static async ValueTask<int> Handle(
					Query _,
					CancellationToken token)
				{
					return 0;
				}
			}

			[Handler]
			public static partial class InternalHandler
			{
				public record Query;
				
				private static async ValueTask<int> Handle(
					Query _,
					{|IAPI0008:GetUsersQuery.Handler|} getUsersQuery,
					CancellationToken token
				) => 0;
			}
			"""
		).RunAsync();

	[Test]
	public async Task HandlerMethodDependsOnNonEndpointHandlerShouldNotWarn() =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<EndpointAsDependencyAnalyzer, ImmediateHandlersGenerator>(
			"""
			using System.Threading;
			using System.Threading.Tasks;
			using Immediate.Apis.Shared;
			using Immediate.Handlers.Shared;
			using Microsoft.AspNetCore.Authorization;
			using Microsoft.AspNetCore.Http;
					
			namespace Dummy;

			[Handler]
			public static partial class GetUsersQuery
			{
				public record Query;

				private static async ValueTask<int> Handle(
					Query _,
					CancellationToken token)
				{
					return 0;
				}
			}

			[Handler]
			public static partial class InternalHandler
			{
				public record Query;
				
				private static async ValueTask<int> Handle(
					Query _,
					GetUsersQuery.Handler getUsersQuery,
					CancellationToken token
				) => 0;
			}
			"""
		).RunAsync();

	[Test]
	public async Task HandlerMethodDependsOnNonHandlerShouldNotWarn() =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<EndpointAsDependencyAnalyzer, ImmediateHandlersGenerator>(
			"""
			using System.Threading;
			using System.Threading.Tasks;
			using Immediate.Apis.Shared;
			using Immediate.Handlers.Shared;
			using Microsoft.AspNetCore.Authorization;
			using Microsoft.AspNetCore.Http;
					
			namespace Dummy;

			public sealed class GetUsersQuery
			{
				public sealed class Handler;
			}

			[Handler]
			public static partial class InternalHandler
			{
				public record Query;
				
				private static async ValueTask<int> Handle(
					Query _,
					GetUsersQuery.Handler getUsersQuery,
					CancellationToken token
				) => 0;
			}
			"""
		).RunAsync();

}
