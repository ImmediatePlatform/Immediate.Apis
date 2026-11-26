using System.Collections.ObjectModel;

namespace Immediate.Apis.Shared;

/// <summary>
///     XML documentation for the request and responses of a generated endpoint.
/// </summary>
/// <param name="parameters">Descriptions keyed by the CLR request member name.</param>
/// <param name="requestBody">The description of the request body.</param>
/// <param name="returns">The default description for successful responses.</param>
/// <param name="responses">Descriptions keyed by OpenAPI response status code.</param>
public sealed class EndpointXmlCommentsMetadata(
	IReadOnlyDictionary<string, string>? parameters,
	string? requestBody,
	string? returns,
	IReadOnlyDictionary<string, string>? responses
)
{
	/// <summary>
	///     Gets the request parameter descriptions keyed by the CLR request member name.
	/// </summary>
	public IReadOnlyDictionary<string, string> Parameters { get; } =
		parameters ?? ReadOnlyDictionary<string, string>.Empty;

	/// <summary>
	///     Gets the description of the request body.
	/// </summary>
	public string? RequestBody { get; } = requestBody;

	/// <summary>
	///     Gets the default description for successful responses.
	/// </summary>
	public string? Returns { get; } = returns;

	/// <summary>
	///     Gets the descriptions keyed by OpenAPI response status code.
	/// </summary>
	public IReadOnlyDictionary<string, string> Responses { get; } =
		responses ?? ReadOnlyDictionary<string, string>.Empty;
}
