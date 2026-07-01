namespace Immediate.Apis.Shared;

/// <summary>
///	    References a <see cref="RouteGroupAttribute"/> marked class to associate this endpoint with a route group.
/// </summary>
/// <typeparam name="T">
///	    The route group class to be associated with.
/// </typeparam>
[AttributeUsage(AttributeTargets.Class)]
public sealed class MapGroupAttribute<T> : Attribute;
