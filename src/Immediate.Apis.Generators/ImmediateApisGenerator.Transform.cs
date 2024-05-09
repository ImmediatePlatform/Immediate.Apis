using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

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

		var httpMethod = attribute.AttributeClass!.Name[..^9];

		if (attribute.ConstructorArguments.FirstOrDefault().Value is not string route)
			return null;

		token.ThrowIfCancellationRequested();

		var allowAnonymous = attributes.Any(a => a.AttributeClass.IsAllowAnonymous());

		var authorizeAttribute = attributes.FirstOrDefault(a => a.AttributeClass.IsAuthorize());
		var authorize = authorizeAttribute != null;
		var authorizePolicy = string.Empty;

		if (authorizeAttribute != null)
		{
			if (authorizeAttribute.ConstructorArguments.Length > 0)
			{
				authorizePolicy = (string)authorizeAttribute.ConstructorArguments[0].Value!;
			}
			else if (authorizeAttribute.NamedArguments.Length > 0)
			{
				foreach (var argument in authorizeAttribute.NamedArguments)
				{
					if (argument.Key != "Policy")
						return null;

					if (argument.Value.Value is not string ap)
						return null;

					authorizePolicy = ap;
				}
			}
		}

		token.ThrowIfCancellationRequested();

		var @namespace = symbol.ContainingNamespace.ToString().NullIf("<global namespace>");
		var @class = GetClass(symbol);
		var className = symbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
		var classAsMethodName = symbol.ToString().Replace(".", "_");
		var parameterType = handleMethod.Parameters[0].Type;

		token.ThrowIfCancellationRequested();

		var parameterAttribute = GetParameterAttribute(handleMethod.Parameters[0], httpMethod);

		token.ThrowIfCancellationRequested();

		var useCustomization = HasCustomizationMethod(symbol);

		token.ThrowIfCancellationRequested();

		var useTransformMethod = HasTransformResultMethod(symbol, handleMethod.ReturnType);

		token.ThrowIfCancellationRequested();

		return new()
		{
			HttpMethod = httpMethod,
			ParameterAttribute = parameterAttribute,
			Route = route,

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
		};
	}

	private static readonly string[] s_methods = ["Get", "Post", "Put", "Patch", "Delete"];
	private static AttributeData? GetMethodAttribute(ImmutableArray<AttributeData> attributes)
	{
		foreach (var name in s_methods)
		{
			var attribute = attributes.FirstOrDefault(a => a.AttributeClass.IsMapMethodAttribute(name));
			if (attribute != null)
				return attribute;
		}

		return null;
	}

	private static IMethodSymbol? GetValidHandleMethod(INamedTypeSymbol symbol)
	{
		if (symbol
				.GetMembers()
				.OfType<IMethodSymbol>()
				.Where(m => m.IsStatic)
				.Where(m => m.Name is "Handle" or "HandleAsync")
				.ToList() is not [var handleMethod])
		{
			return null;
		}

		// must have request type and cancellation token
		if (handleMethod.Parameters.Length < 2)
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
				&& paramType.IsIEndpointConventionBuilder()
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

		if (parameterSymbol is INamedTypeSymbol typeSymbol)
		{
			foreach (var p in typeSymbol.GetMembers().OfType<IPropertySymbol>())
			{
				if (p.Type.IsIFormFile())
					return "FromForm";
			}
		}

		return httpMethod is "MapGet" or "MapDelete"
			? "AsParameters"
			: "FromBody";
	}
}
