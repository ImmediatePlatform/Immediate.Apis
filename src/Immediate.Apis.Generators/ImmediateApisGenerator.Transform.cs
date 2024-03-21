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
		var attributeNames = attributes
			.Select(a => a.AttributeClass?.ToString() ?? "")
			.ToList();

		token.ThrowIfCancellationRequested();

		if (GetMethodAttributeIndex(attributeNames) is not { } methodIndex)
			return null;

		if (GetValidHandleMethod(symbol) is not { } handleMethod)
			return null;

		token.ThrowIfCancellationRequested();

		var attribute = attributes[methodIndex];
		var httpMethod = attribute.AttributeClass!.Name[..^9];
		var route = (string?)attribute.ConstructorArguments.FirstOrDefault().Value;

		if (route == null)
			return null;

		token.ThrowIfCancellationRequested();

		var allowAnonymous = attributeNames.Contains("Microsoft.AspNetCore.Authorization.AllowAnonymousAttribute");

		var authorizeIndex = attributeNames.IndexOf("Microsoft.AspNetCore.Authorization.AuthorizeAttribute");
		var authorize = authorizeIndex >= 0;
		var authorizePolicy = string.Empty;

		if (authorize)
		{
			var authorizeAttribute = attributes[authorizeIndex];
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

					authorizePolicy = (string)argument.Value.Value!;
				}
			}
		}

		token.ThrowIfCancellationRequested();

		var className = symbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
		var classAsMethodName = symbol.ToString().Replace(".", "_");
		var parameterType = handleMethod.Parameters[0].Type
			.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

		token.ThrowIfCancellationRequested();

		var useCustomization = HasCustomizationMethod(symbol);

		token.ThrowIfCancellationRequested();

		return new()
		{
			HttpMethod = httpMethod,
			Route = route,

			ClassName = className,
			ClassAsMethodName = classAsMethodName,
			ParameterType = parameterType,

			AllowAnonymous = allowAnonymous,
			Authorize = authorize,
			AuthorizePolicy = authorizePolicy,

			UseCustomization = useCustomization,
		};
	}

	private static int? GetMethodAttributeIndex(List<string> attributeNames)
	{
		foreach (var name in s_methodAttributes)
		{
			var index = attributeNames.IndexOf(name);
			if (index >= 0)
				return index;
		}

		return null;
	}

	private static IMethodSymbol? GetValidHandleMethod(INamedTypeSymbol symbol)
	{
		if (symbol
				.GetMembers()
				.OfType<IMethodSymbol>()
				.Where(m => m.IsStatic)
				.Where(m =>
					m.Name.Equals("Handle", StringComparison.Ordinal)
					|| m.Name.Equals("HandleAsync", StringComparison.Ordinal)
				)
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
			.Any(m => m.IsStatic
				&& m.DeclaredAccessibility == Accessibility.Public
				&& m.Name.Equals("CustomizeEndpoint", StringComparison.Ordinal)
				&& m.Parameters.Length == 1
				&& m.Parameters[0].Type.ToString() == "Microsoft.AspNetCore.Builder.IEndpointConventionBuilder"
				&& m.ReturnsVoid
			);
}
