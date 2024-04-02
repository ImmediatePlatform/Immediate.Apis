namespace Immediate.Apis.Shared;

/// <summary>
///		Common Registrations for use with <see cref="EndpointRegistrationOverrideAttribute"/>.
/// </summary>
public static class EndpointRegistration
{
	/// <summary>
	///		Use <c>[AsParameters]</c> as the registration
	/// </summary>
	public const string AsParameters = nameof(AsParameters);

	/// <summary>
	///		Use <c>[FromBody]</c> as the registration
	/// </summary>
	public const string FromBody = nameof(FromBody);

	/// <summary>
	///		Use <c>[FromForm]</c> as the registration
	/// </summary>
	public const string FromForm = nameof(FromForm);

	/// <summary>
	///		Use <c>[FromHeader]</c> as the registration
	/// </summary>
	public const string FromHeader = nameof(FromHeader);

	/// <summary>
	///		Use <c>[FromQuery]</c> as the registration
	/// </summary>
	public const string FromQuery = nameof(FromQuery);

	/// <summary>
	///		Use <c>[FromRoute]</c> as the registration
	/// </summary>
	public const string FromRoute = nameof(FromRoute);
}

/// <summary>
///		Specify how the handler parameter should be registered with Minimal Apis
/// </summary>
/// <param name="registration">
///		The desired registration
/// </param>
[AttributeUsage(AttributeTargets.Class)]
public sealed class EndpointRegistrationOverrideAttribute(
	string registration
) : Attribute
{
	/// <summary>
	///		The desired registration
	/// </summary>
	public string Registration { get; } = registration;
}
