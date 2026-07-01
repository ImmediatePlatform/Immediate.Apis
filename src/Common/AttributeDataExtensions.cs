using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace Immediate.Apis;

internal static class AttributeDataExtensions
{
	extension(ImmutableArray<AttributeData> attributes)
	{
		public AttributeData? GetMethodAttribute() =>
			attributes.FirstOrDefault(a => a.AttributeClass.IsMapMethodAttribute);

		public AttributeData? GetRouteGroupAttribute() =>
			attributes.FirstOrDefault(a => a.AttributeClass.IsRouteGroupAttribute);

		public AttributeData? GetMapGroupAttribute() =>
			attributes.FirstOrDefault(a => a.AttributeClass.IsMapGroupAttribute);
	}

	extension(AttributeData attributeData)
	{
		public IReadOnlyList<string> GetRoutes()
		{
			return attributeData switch
			{
				{
					AttributeClass.Name: "MapMethodAttribute",
					ConstructorArguments:
					[
					_,
					{ Kind: TypedConstantKind.Array, Values: var arr },
					],
				} => [.. arr.Select(a => a.Value).OfType<string>()],

				{
					ConstructorArguments:
					[
					{ Kind: TypedConstantKind.Array, Values: var arr },
					],
				} => [.. arr.Select(a => a.Value).OfType<string>()],

				{
					ConstructorArguments:
					[
					{ Kind: TypedConstantKind.Primitive, Value: string str },
					],
				} => [str],

				_ => [],
			};
		}

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

		public string? GetRouteGroup() =>
			attributeData switch
			{
				{
					AttributeClass.Name: "RouteGroupAttribute",
					ConstructorArguments:
					[
					{ Value: string routeGroup },
					],
				} => routeGroup,

				_ => null,
			};
	}
}
