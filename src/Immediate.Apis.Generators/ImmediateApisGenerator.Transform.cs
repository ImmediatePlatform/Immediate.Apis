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
		var displayName = symbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

		token.ThrowIfCancellationRequested();

		if (symbol.ContainingType is not null)
			return null;

		token.ThrowIfCancellationRequested();

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

		token.ThrowIfCancellationRequested();

		var requestType = handleMethod.Parameters[0].Type
			.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

		token.ThrowIfCancellationRequested();

		var attributeNames = symbol.GetAttributes()
			.Select(a => a.AttributeClass?.ToString() ?? "")
			.ToList();

		foreach (var methodName in s_methodAttributes)
		{
			var methodIndex = attributeNames.IndexOf(methodName);
			if (methodIndex < 0)
				continue;

			token.ThrowIfCancellationRequested();

			var attribute = symbol.GetAttributes()[methodIndex];
			var method = attribute.AttributeClass!.Name[..^9];
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
				var authorizeAttribute = symbol.GetAttributes()[authorizeIndex];
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

			return new()
			{
				Route = route,
				ClassName = displayName,
				MethodName = method,
				ParameterType = requestType,
				AllowAnonymous = allowAnonymous,
				Authorize = authorize,
				AuthorizePolicy = authorizePolicy
			};
		}

		return null;
	}
}
