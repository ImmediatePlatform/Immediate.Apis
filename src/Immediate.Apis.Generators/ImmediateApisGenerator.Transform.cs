using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Immediate.Apis.Generators;

public sealed partial class ImmediateApisGenerator
{
	private static Method? TransformMethod(
		GeneratorAttributeSyntaxContext context,
		CancellationToken token
	)
	{
		token.ThrowIfCancellationRequested();

		var symbol = (INamedTypeSymbol)context.TargetSymbol;
		var attributes = symbol.GetAttributes();

		token.ThrowIfCancellationRequested();

		if (GetMethodAttribute(attributes) is not { } attribute)
			return null;

		if (GetValidHandleMethod(symbol) is not { } handleMethod)
			return null;

		token.ThrowIfCancellationRequested();

		var routes = GetRoutes(attribute);
		if (routes.Count == 0)
			return null;

		token.ThrowIfCancellationRequested();

		var allowAnonymous = attributes.Any(a => a.AttributeClass.IsAllowAnonymous());

		var authorizeAttribute = attributes.FirstOrDefault(a => a.AttributeClass.IsAuthorize());
		var authorize = authorizeAttribute != null;
		var authorizePolicy = string.Empty;

		switch (authorizeAttribute)
		{
			case { ConstructorArguments.Length: > 0 }:
				authorizePolicy = (string)authorizeAttribute.ConstructorArguments[0].Value!;
				break;

			case { NamedArguments.Length: > 0 }:
			{
				foreach (var argument in authorizeAttribute.NamedArguments)
				{
					if (argument is not { Key: "Policy", Value.Value: string ap })
						return null;

					authorizePolicy = ap;
				}

				break;
			}

			default:
				break;
		}

		token.ThrowIfCancellationRequested();

		var @namespace = symbol.ContainingNamespace.ToString().NullIf("<global namespace>");
		var @class = GetClass(symbol);

		token.ThrowIfCancellationRequested();

		var className = symbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

		token.ThrowIfCancellationRequested();

		var classAsMethodName = symbol.ToString().Replace('.', '_');

		token.ThrowIfCancellationRequested();

		var parameterType = handleMethod.Parameters[0].Type;
		var mapMethod = GetMapMethod(attribute);

		token.ThrowIfCancellationRequested();

		var httpMethod = GetHttpMethod(attribute);

		token.ThrowIfCancellationRequested();

		var parameterAttribute = GetParameterAttribute(handleMethod.Parameters[0], mapMethod);

		token.ThrowIfCancellationRequested();

		var handleMethodAttributes = GetHandleMethodAttributes(handleMethod);

		token.ThrowIfCancellationRequested();

		var useCustomization = HasCustomizationMethod(symbol);

		token.ThrowIfCancellationRequested();

		var useTransformMethod = HasTransformResultMethod(symbol, handleMethod.ReturnType);

		token.ThrowIfCancellationRequested();

		return new()
		{
			MapMethod = mapMethod,
			HttpMethod = httpMethod,
			Attributes = handleMethodAttributes,
			ParameterAttribute = parameterAttribute,
			Routes = routes,

			Namespace = @namespace,
			Class = @class,
			ClassFullName = className,
			ClassAsMethodName = classAsMethodName,
			ParameterType = parameterType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),

			AllowAnonymous = allowAnonymous,
			Authorize = authorize,
			AuthorizePolicy = authorizePolicy,

			UseCustomization = useCustomization,
			UseTransformMethod = useTransformMethod,
			HasReturn = handleMethod.ReturnType.IsValueTask1(),
		};
	}

	private static AttributeData? GetMethodAttribute(ImmutableArray<AttributeData> attributes) =>
		attributes.FirstOrDefault(a => a.AttributeClass.IsMapMethodAttribute());

	private static IMethodSymbol? GetValidHandleMethod(INamedTypeSymbol symbol)
	{
		if (symbol
				.GetMembers()
				.OfType<IMethodSymbol>()
				.Where(m => m.Name is "Handle" or "HandleAsync")
				.Take(2)
				.ToList() is not [var handleMethod])
		{
			return null;
		}

		// must have request type
		if (handleMethod.Parameters.Length is 0)
			return null;

		return handleMethod;
	}

	private static bool HasCustomizationMethod(INamedTypeSymbol symbol)
		=> symbol
			.GetMembers()
			.OfType<IMethodSymbol>()
			.Any(m =>
				m is
				{
					Name: "CustomizeEndpoint",
					IsStatic: true,
					DeclaredAccessibility: Accessibility.Internal,
					ReturnsVoid: true,
					Parameters: [{ Type: { } paramType }],
				}
				&& paramType.IsIEndpointConventionBuilderOrRouteHandlerBuilder()
			);

	private static bool HasTransformResultMethod(INamedTypeSymbol symbol, ITypeSymbol returnType)
		=> returnType.IsValueTask1()
			&& returnType is INamedTypeSymbol { TypeArguments: [{ } returnInnerType] }
			&& symbol
				.GetMembers()
				.OfType<IMethodSymbol>()
				.Any(m =>
					m is
					{
						Name: "TransformResult",
						IsStatic: true,
						DeclaredAccessibility: Accessibility.Internal,
						ReturnsVoid: false,
						Parameters: [{ Type: { } paramType }],
					}
					&& SymbolEqualityComparer.IncludeNullability.Equals(returnInnerType, paramType)
				);

	private static Class GetClass(INamedTypeSymbol symbol) =>
		new()
		{
			Name = symbol.Name,
			Type = symbol switch
			{
				{ TypeKind: TypeKind.Interface } => "interface",
				{ IsRecord: true, TypeKind: TypeKind.Struct, } => "record struct",
				{ IsRecord: true, } => "record",
				{ TypeKind: TypeKind.Struct, } => "struct",
				_ => "class",
			},
		};

	private static string GetParameterAttribute(IParameterSymbol parameterSymbol, string httpMethod)
	{
		foreach (var a in parameterSymbol.GetAttributes())
		{
			if (a.AttributeClass.IsBindingParameterAttribute())
				return a.AttributeClass.Name[..^9];
		}

		if (parameterSymbol.Type is INamedTypeSymbol typeSymbol)
		{
			foreach (var p in typeSymbol.GetMembers().OfType<IPropertySymbol>())
			{
				if (p.Type.IsIFormFile())
					return "FromForm";
			}

			foreach (var p in typeSymbol.GetMembers().OfType<IPropertySymbol>())
			{
				if (p.GetAttributes().Any(a => a.AttributeClass.IsFromXxxAttribute()))
					return "AsParameters";
			}
		}

		return httpMethod is "MapPatch" or "MapPost" or "MapPut"
			? "FromBody"
			: "AsParameters";
	}

	private static string GetMapMethod(AttributeData attributeData)
	{
		var attributeName = attributeData.AttributeClass!.Name;

		if (attributeName is "MapMethodAttribute")
			return "MapMethods";

		return attributeName[..^9];
	}

	private static string? GetHttpMethod(AttributeData attributeData)
	{
		var attributeName = attributeData.AttributeClass!.Name;

		if (attributeName is not "MapMethodAttribute")
			return null;

		if (attributeData.ConstructorArguments.Length < 2)
			return null;

		var secondArgument = attributeData.ConstructorArguments[1];

		// If second argument is an array, it's the new constructor (method at [0])
		if (secondArgument.Kind == TypedConstantKind.Array)
		{
			return attributeData
				.ConstructorArguments[0]
				.Value
				?.ToString();
		}

		// Otherwise it's the old constructor (method at [1])
		return attributeData
			.ConstructorArguments[1]
			.Value
			?.ToString();
	}

	private static EquatableReadOnlyList<string> GetHandleMethodAttributes(IMethodSymbol methodSymbol) =>
		methodSymbol.GetAttributes()
			.Select(GetAttributeString)
			.ToEquatableReadOnlyList();

	private static string GetAttributeString(AttributeData attributeData)
	{
		var @class = attributeData.AttributeClass!.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

		var parameters = new List<string>();

		foreach (var tc in attributeData.ConstructorArguments)
		{
			if (GetTypedConstantString(tc) is { } str)
				parameters.Add(str);
		}

		foreach (var na in attributeData.NamedArguments)
		{
			if (GetTypedConstantString(na.Value) is { } str)
				parameters.Add($"{na.Key} = {str}");
		}

		return parameters.Count == 0
			? @class
			: $"{@class}({string.Join(", ", parameters)})";
	}

	[SuppressMessage("Style", "IDE0072:Add missing cases")]
	private static string? GetTypedConstantString(TypedConstant tc) =>
		tc.Kind switch
		{
			TypedConstantKind.Array => $"[{string.Join(", ", tc.Values.Select(GetTypedConstantString))}]",
			_ => tc.ToCSharpString(),
		};

	private static EquatableReadOnlyList<string> GetRoutes(AttributeData attributeData)
	{
		var attributeName = attributeData.AttributeClass!.Name;

		if (attributeName is "MapMethodAttribute")
		{
			// MapMethodAttribute has two constructors:
			// 1. Old: MapMethodAttribute(string route, string method) - route at [0], method at [1]
			// 2. New: MapMethodAttribute(string method, params string[] routes) - method at [0], routes at [1]

			if (attributeData.ConstructorArguments.Length < 2)
				return new EquatableReadOnlyList<string>([]);

			var secondArgument = attributeData.ConstructorArguments[1];

			// If second argument is an array, it's the new constructor (routes at [1])
			if (secondArgument.Kind == TypedConstantKind.Array)
			{
				var routes = new List<string>();
				foreach (var routeValue in secondArgument.Values)
				{
					if (routeValue.Value is string route)
						routes.Add(route);
				}

				return routes.ToEquatableReadOnlyList();
			}
			// Otherwise it's the old constructor (route at [0])

			if (attributeData.ConstructorArguments[0].Value is string oldCtorRoute)
			{
				return new EquatableReadOnlyList<string>([oldCtorRoute]);
			}

			return new EquatableReadOnlyList<string>([]);
		}

		// For derived attributes (MapGet, MapPost, etc.), routes are in constructor argument [0]
		if (attributeData.ConstructorArguments.Length == 0)
			return new EquatableReadOnlyList<string>([]);

		var routesArgument = attributeData.ConstructorArguments[0];

		if (routesArgument.Kind != TypedConstantKind.Array)
			return new EquatableReadOnlyList<string>([]);

		var derivedAttrRoutes = new List<string>();
		foreach (var routeValue in routesArgument.Values)
		{
			if (routeValue.Value is string route)
				derivedAttrRoutes.Add(route);
		}

		return derivedAttrRoutes.ToEquatableReadOnlyList();
	}
}
