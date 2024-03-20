namespace Immediate.Apis.FunctionalTests.Filters;

public sealed class ApiKeyEndpointFilter : IEndpointFilter
{
	public const string ApiKey = "123456";

	public ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
	{
		ArgumentNullException.ThrowIfNull(context, nameof(context));
		ArgumentNullException.ThrowIfNull(next, nameof(next));

		if (!context.HttpContext.Request.Headers.TryGetValue("X-Api-Key", out var apiKey))
		{
			context.HttpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
			return new ValueTask<object?>(Task.CompletedTask);
		}

		if (apiKey == ApiKey) return next(context);

		context.HttpContext.Response.StatusCode = StatusCodes.Status403Forbidden;
		return new ValueTask<object?>(Task.CompletedTask);
	}
}
