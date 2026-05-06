using Microsoft.CodeAnalysis;

namespace Immediate.Apis;

internal static class AttributeDataExtensions
{
	extension(AttributeData attributeData)
	{
		public string? GetHttpMethod() =>
			attributeData.GetMapMethodMethod()
			?? attributeData.GetMapMethodName()[3..];

		public string GetMapMethodName()
		{
			var attributeName = attributeData.AttributeClass!.Name;

			if (attributeName is "MapMethodAttribute")
				return "MapMethods";

			return attributeName[..^9];
		}

		public string? GetMapMethodMethod() =>
			attributeData switch
			{
				{
					AttributeClass.Name: "MapMethodAttribute",
					ConstructorArguments:
					[
					{ Value: { } method },
					{ Kind: TypedConstantKind.Array },
					],
				} => method.ToString(),

				_ => null,
			};
	}
}
