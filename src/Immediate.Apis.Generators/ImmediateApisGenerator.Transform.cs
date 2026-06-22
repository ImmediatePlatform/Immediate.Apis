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

		if (attributes.GetMethodAttribute() is not { } attribute)
			return null;

		if (attribute.GetRoutes() is not { Count: > 0 } routes)
			return null;

		token.ThrowIfCancellationRequested();

		if (GetValidHandleMethod(symbol) is not { } handleMethod)
			return null;

		token.ThrowIfCancellationRequested();

		var allowAnonymous = attributes.Any(a => a.AttributeClass.IsAllowAnonymousAttribute);

		var authorizeAttribute = attributes.FirstOrDefault(a => a.AttributeClass.IsAuthorizeAttribute);
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
		var classAsMethodName = symbol.ToString().Replace('.', '_');
		var parameterType = handleMethod.Parameters[0].Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

		token.ThrowIfCancellationRequested();

		var mapMethod = attribute.GetMapMethodName();
		var httpMethod = attribute.GetMapMethodMethod();
		var parameterAttribute = GetParameterAttribute(handleMethod.Parameters[0], mapMethod);
		var handleMethodAttributes = GetHandleMethodAttributes(handleMethod);
		var useCustomization = HasCustomizationMethod(symbol);
		var useTransformMethod = HasTransformResultMethod(symbol, handleMethod.ReturnType);
		var routeGroup = GetRouteGroupName(attributes);

		token.ThrowIfCancellationRequested();

		return new()
		{
			MapMethod = mapMethod,
			HttpMethod = httpMethod,
			Attributes = handleMethodAttributes,
			ParameterAttribute = parameterAttribute,
			Routes = new(routes),

			Namespace = @namespace,
			Class = @class,
			ClassFullName = className,
			ClassAsMethodName = classAsMethodName,
			ParameterType = parameterType,

			AllowAnonymous = allowAnonymous,
			Authorize = authorize,
			AuthorizePolicy = authorizePolicy,

			UseCustomization = useCustomization,
			UseTransformMethod = useTransformMethod,
			HasReturn = handleMethod.ReturnType.IsValueTask1,

			RouteGroupName = routeGroup,
		};
	}

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
				&& paramType.IsIEndpointConventionBuilderOrRouteHandlerBuilder
			);

	private static bool HasTransformResultMethod(INamedTypeSymbol symbol, ITypeSymbol returnType)
		=> returnType.IsValueTask1
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
			if (a.AttributeClass.IsBindingParameterAttribute)
				return a.AttributeClass.Name[..^9];
		}

		if (parameterSymbol.Type is INamedTypeSymbol typeSymbol)
		{
			foreach (var p in typeSymbol.GetMembers().OfType<IPropertySymbol>())
			{
				if (p.Type.IsIFormFile)
					return "FromForm";
			}

			foreach (var p in typeSymbol.GetMembers().OfType<IPropertySymbol>())
			{
				if (p.GetAttributes().Any(a => a.AttributeClass.IsFromXxxAttribute))
					return "AsParameters";
			}
		}

		return httpMethod is "MapPatch" or "MapPost" or "MapPut"
			? "FromBody"
			: "AsParameters";
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

	private static string? GetRouteGroupName(ImmutableArray<AttributeData> attributes)
	{
		return attributes.GetRouteGroupAttribute() switch
		{
			{ } attribute => attribute.GetRouteGroup(),
			_ => null,
		};
}
}
