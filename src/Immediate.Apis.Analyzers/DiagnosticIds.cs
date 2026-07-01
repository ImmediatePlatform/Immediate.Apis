namespace Immediate.Apis.Analyzers;

internal static class DiagnosticIds
{
	public const string IAPI0001MissingHandlerAttribute = "IAPI0001";
	public const string IAPI0002InvalidAuthorizeParameter = "IAPI0002";
	public const string IAPI0003UsedBothAuthorizeAndAnonymous = "IAPI0003";
	public const string IAPI0004CustomizeEndpointInvalid = "IAPI0004";
	public const string IAPI0005TransformResultInvalid = "IAPI0005";
	public const string IAPI0006MissingCustomizeEndpointMethod = "IAPI0006";
	public const string IAPI0007MissingTransformResultMethod = "IAPI0007";
	public const string IAPI0008HandlerShouldNotDependOnEndpoint = "IAPI0008";
	public const string IAPI0009EndpointHasBeenSpecifiedMultipleTimes = "IAPI0009";
	public const string IAPI0010InvalidRouteGroupName = "IAPI0010";
	public const string IAPI0011CustomizeGroupInvalid = "IAPI0011";
	public const string IAPI0012MissingCustomizeGroupMethod = "IAPI0012";
}
