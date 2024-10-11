//HintName: IH.Dummy.GetUserQuery.g.cs
using Microsoft.Extensions.DependencyInjection;

#pragma warning disable CS1591

namespace Dummy;

partial class GetUserQuery
{
	public sealed partial class Handler : global::Immediate.Handlers.Shared.IHandler<global::Dummy.GetUserQuery.Query, int>
	{
		private readonly global::Dummy.GetUserQuery.HandleBehavior _handleBehavior;

		public Handler(
			global::Dummy.GetUserQuery.HandleBehavior handleBehavior
		)
		{
			var handlerType = typeof(GetUserQuery);

			_handleBehavior = handleBehavior;

		}

		public async global::System.Threading.Tasks.ValueTask<int> HandleAsync(
			global::Dummy.GetUserQuery.Query request,
			global::System.Threading.CancellationToken cancellationToken = default
		)
		{
			return await _handleBehavior
				.HandleAsync(request, cancellationToken)
				.ConfigureAwait(false);
		}
	}

	[global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
	public sealed class HandleBehavior : global::Immediate.Handlers.Shared.Behavior<global::Dummy.GetUserQuery.Query, int>
	{

		public HandleBehavior(
		)
		{
		}

		public override async global::System.Threading.Tasks.ValueTask<int> HandleAsync(
			global::Dummy.GetUserQuery.Query request,
			global::System.Threading.CancellationToken cancellationToken
		)
		{
			return await global::Dummy.GetUserQuery
				.HandleAsync(
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
		services.Add(new(typeof(global::Dummy.GetUserQuery.Handler), typeof(global::Dummy.GetUserQuery.Handler), lifetime));
		services.Add(new(typeof(global::Immediate.Handlers.Shared.IHandler<global::Dummy.GetUserQuery.Query, int>), typeof(global::Dummy.GetUserQuery.Handler), lifetime));
		services.Add(new(typeof(global::Dummy.GetUserQuery.HandleBehavior), typeof(global::Dummy.GetUserQuery.HandleBehavior), lifetime));
		return services;
	}
}
