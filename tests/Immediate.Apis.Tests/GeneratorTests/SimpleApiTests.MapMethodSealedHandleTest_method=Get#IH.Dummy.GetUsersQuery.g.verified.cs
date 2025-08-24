﻿//HintName: IH.Dummy.GetUsersQuery.g.cs
using Microsoft.Extensions.DependencyInjection;

#pragma warning disable CS1591

namespace Dummy;

partial class GetUsersQuery
{
	public sealed partial class Handler : global::Immediate.Handlers.Shared.IHandler<global::Dummy.GetUsersQuery.Query, int>
	{
		private readonly global::Dummy.GetUsersQuery.HandleBehavior _handleBehavior;

		public Handler(
			global::Dummy.GetUsersQuery.HandleBehavior handleBehavior
		)
		{
			var handlerType = typeof(GetUsersQuery);

			_handleBehavior = handleBehavior;

		}

		public async global::System.Threading.Tasks.ValueTask<int> HandleAsync(
			global::Dummy.GetUsersQuery.Query request,
			global::System.Threading.CancellationToken cancellationToken = default
		)
		{
			return await _handleBehavior
				.HandleAsync(request, cancellationToken)
				.ConfigureAwait(false);
		}
	}

	[global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
	public sealed class HandleBehavior : global::Immediate.Handlers.Shared.Behavior<global::Dummy.GetUsersQuery.Query, int>
	{
		private readonly global::Dummy.GetUsersQuery _container;

		public HandleBehavior(
			global::Dummy.GetUsersQuery container
		)
		{
			_container = container;
		}

		public override async global::System.Threading.Tasks.ValueTask<int> HandleAsync(
			global::Dummy.GetUsersQuery.Query request,
			global::System.Threading.CancellationToken cancellationToken
		)
		{
			return await _container
				.Handle(
					request
					, cancellationToken
				)
				.ConfigureAwait(false);
		}
	}

	[global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
	public static IServiceCollection AddHandlers(
		IServiceCollection services,
		ServiceLifetime lifetime = ServiceLifetime.Scoped
	)
	{
		services.Add(new(typeof(global::Dummy.GetUsersQuery.Handler), typeof(global::Dummy.GetUsersQuery.Handler), lifetime));
		services.Add(new(typeof(global::Immediate.Handlers.Shared.IHandler<global::Dummy.GetUsersQuery.Query, int>), typeof(global::Dummy.GetUsersQuery.Handler), lifetime));
		services.Add(new(typeof(global::Dummy.GetUsersQuery.HandleBehavior), typeof(global::Dummy.GetUsersQuery.HandleBehavior), lifetime));
		services.Add(new(typeof(global::Dummy.GetUsersQuery), typeof(global::Dummy.GetUsersQuery), lifetime));
		return services;
	}
}
