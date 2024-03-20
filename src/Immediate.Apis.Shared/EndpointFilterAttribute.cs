using Microsoft.AspNetCore.Http;

namespace Immediate.Apis.Shared;

/// <summary>
///     Applied to a class to specify an endpoint filter that should be applied to the minimal API endpoints within the class.
/// </summary>
/// <typeparam name="T">
///     The type of the endpoint filter to apply. Must implement the <see cref="IEndpointFilter"/> interface.
/// </typeparam>
/// <remarks>
///     <para>
///         This attribute can be applied multiple times to a class to specify multiple endpoint filters.
///     </para>
///     <para>
///         The endpoint filters will be applied in the order they are listed on the class. The order of the attributes determines
///         the order in which the filters are executed during the request processing pipeline.
///     </para>
/// </remarks>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public sealed class EndpointFilterAttribute<T> : Attribute
	where T : IEndpointFilter;
