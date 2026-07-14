using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Immediate.Apis.Generators;

public sealed partial class ImmediateApisGenerator
{
	private static Method? TransformEndpoint(
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

		if (symbol.GetValidHandleMethod() is not { } handleMethod)
			return null;

		token.ThrowIfCancellationRequested();

		if (!TryGetMapGoup(symbol, out var routeGroupFullClassName))
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

		var @namespace = symbol.ContainingNamespace.ToDisplayString().NullIf("<global namespace>");
		var @class = GetClass(symbol);

		token.ThrowIfCancellationRequested();

		var className = symbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
		var classAsMethodName = symbol.ToDisplayString().Replace('.', '_');

		var parameterType = handleMethod.Parameters[0].Type;

		token.ThrowIfCancellationRequested();

		var mapMethod = attribute.GetMapMethodName();
		var httpMethod = attribute.GetMapMethodMethod();
		var parameterAttribute = GetParameterAttribute(handleMethod.Parameters[0], mapMethod);
		var handleMethodAttributes = GetHandleMethodAttributes(handleMethod);
		var useCustomization = HasCustomizeEndpointMethod(symbol);
		var useTransformMethod = HasTransformResultMethod(symbol, handleMethod.ReturnType);

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

			ParameterType = parameterType.ToDisplayString(DisplayNameFormatters.FullyQualifiedWithNullableFormat),
			ParameterTypeDoc = parameterType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),

			AllowAnonymous = allowAnonymous,
			Authorize = authorize,
			AuthorizePolicy = authorizePolicy,

			UseCustomization = useCustomization,
			UseTransformMethod = useTransformMethod,
			HasReturn = handleMethod.ReturnType.IsValueTask1,

			RouteGroupClassFullName = routeGroupFullClassName,
		};
	}

	private static RouteGroupDefinition? TransformRouteGroup(
		GeneratorAttributeSyntaxContext context,
		CancellationToken token
	)
	{
		token.ThrowIfCancellationRequested();

		if (context.Attributes[0].ConstructorArguments is not [{ Value: string route }])
			return null;

		var symbol = (INamedTypeSymbol)context.TargetSymbol;

		if (symbol.ContainingType is { } && symbol.ContainingType.GetAttributes().GetRouteGroupAttribute() is null)
			return null;

		var @namespace = symbol.ContainingNamespace.ToDisplayString().NullIf("<global namespace>");
		var outerClasses = GetOuterClasses(symbol);
		var @class = GetClass(symbol);
		var customization = HasCustomizeGroupMethod(symbol);

		return new()
		{
			Namespace = @namespace,
			OuterClasses = outerClasses,
			Class = @class,
			ClassFullName = symbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
			RouteGroupClassFullName = symbol.ContainingType?.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
			Route = route,
			UseCustomization = customization,
		};
	}

	private static bool TryGetMapGoup(INamedTypeSymbol symbol, out string? routeGroupFullClassName)
	{
		if (symbol.GetAttributes().GetMapGroupAttribute() is not { AttributeClass.TypeArguments: [{ } groupTypeSymbol] })
		{
			routeGroupFullClassName = null;
			return true;
		}

		if (groupTypeSymbol.GetAttributes().GetRouteGroupAttribute() is null)
		{
			routeGroupFullClassName = null;
			return false;
		}

		routeGroupFullClassName = groupTypeSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
		return true;
	}

	private static bool HasCustomizeEndpointMethod(INamedTypeSymbol symbol)
		=> symbol
			.GetMembers()
			.OfType<IMethodSymbol>()
			.Any(m =>
				m is
				{
					Name: "CustomizeEndpoint",
					IsStatic: true,
					DeclaredAccessibility: Accessibility.Internal or Accessibility.Private,
					ReturnsVoid: true,
					Parameters: [{ Type.IsIEndpointConventionBuilderOrRouteHandlerBuilder: true }],
				}
			);

	private static bool HasCustomizeGroupMethod(INamedTypeSymbol symbol)
		=> symbol
			.GetMembers()
			.OfType<IMethodSymbol>()
			.Any(m =>
				m is
				{
					Name: "CustomizeGroup",
					IsStatic: true,
					DeclaredAccessibility: Accessibility.Private,
					ReturnsVoid: true,
					Parameters: [{ Type.IsRouteGroupBuilder: true }],
				}
			);

	private static bool HasTransformResultMethod(INamedTypeSymbol symbol, ITypeSymbol returnType)
	{
		return (
			returnType is INamedTypeSymbol { IsValueTask1: true, TypeArguments: [{ } returnInnerType] }
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
				)
		)
		|| (
			returnType is INamedTypeSymbol { IsValueTask: true }
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
						Parameters: [],
					}
				)
				);
	}

	private static EquatableReadOnlyList<Class> GetOuterClasses(INamedTypeSymbol symbol)
	{
		List<Class>? outerClasses = null;
		var outerSymbol = symbol.ContainingType;
		while (outerSymbol is not null)
		{
			(outerClasses ??= []).Add(GetClass(outerSymbol));
			outerSymbol = outerSymbol.ContainingType;
		}

		if (outerClasses is null)
			return default;

		outerClasses.Reverse();

		return outerClasses.ToEquatableReadOnlyList();
	}

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
				return a.AttributeClass.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
		}

		if (parameterSymbol.Type is INamedTypeSymbol typeSymbol)
		{
			foreach (var p in typeSymbol.GetMembers().OfType<IPropertySymbol>())
			{
				if (p.Type.IsIFormFile)
					return "global::Microsoft.AspNetCore.Mvc.FromFormAttribute";
			}

			foreach (var p in typeSymbol.GetMembers().OfType<IPropertySymbol>())
			{
				if (p.GetAttributes().Any(a => a.AttributeClass.IsFromXxxAttribute))
					return "global::Microsoft.AspNetCore.Http.AsParametersAttribute";
			}
		}

		return httpMethod is "MapPatch" or "MapPost" or "MapPut"
			? "global::Microsoft.AspNetCore.Mvc.FromBodyAttribute"
			: "global::Microsoft.AspNetCore.Http.AsParametersAttribute";
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
}
