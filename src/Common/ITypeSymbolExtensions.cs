using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis;

namespace Immediate.Apis;

internal static class ITypeSymbolExtensions
{
	public static bool IsMapMethodAttribute(this ITypeSymbol? typeSymbol) =>
		typeSymbol is INamedTypeSymbol
		{
			Arity: 0,
			Name: "MapGetAttribute"
				or "MapPostAttribute"
				or "MapPutAttribute"
				or "MapPatchAttribute"
				or "MapDeleteAttribute"
				or "MapMethodAttribute",
			ContainingNamespace.IsImmediateApisShared: true,
		};

	public static bool IsAllowAnonymous(this ITypeSymbol? typeSymbol) =>
		typeSymbol is INamedTypeSymbol
		{
			Arity: 0,
			Name: "AllowAnonymousAttribute",
			ContainingNamespace.IsMicrosoftAspNetCoreAuthorization: true,
		};

	public static bool IsAuthorize(this ITypeSymbol? typeSymbol) =>
		typeSymbol is INamedTypeSymbol
		{
			Arity: 0,
			Name: "AuthorizeAttribute",
			ContainingNamespace.IsMicrosoftAspNetCoreAuthorization: true,
		};

	public static bool IsHandlerAttribute(this ITypeSymbol? typeSymbol) =>
		typeSymbol is
		{
			Name: "HandlerAttribute",
			ContainingNamespace:
			{
				Name: "Shared",
				ContainingNamespace:
				{
					Name: "Handlers",
					ContainingNamespace:
					{
						Name: "Immediate",
						ContainingNamespace.IsGlobalNamespace: true,
					},
				},
			},
		};

	public static bool IsIEndpointConventionBuilderOrRouteHandlerBuilder(this ITypeSymbol? typeSymbol) =>
		typeSymbol is INamedTypeSymbol
		{
			Arity: 0,
			Name: "IEndpointConventionBuilder" or "RouteHandlerBuilder",
			ContainingNamespace.IsMicrosoftAspNetCoreBuilder: true,
		};

	public static bool IsValueTask1(this ITypeSymbol? typeSymbol) =>
		typeSymbol is INamedTypeSymbol
		{
			Arity: 1,
			Name: "ValueTask",
			ContainingNamespace:
			{
				Name: "Tasks",
				ContainingNamespace:
				{
					Name: "Threading",
					ContainingNamespace:
					{
						Name: "System",
						ContainingNamespace.IsGlobalNamespace: true,
					},
				},
			},
		};

	public static bool IsBindingParameterAttribute([NotNullWhen(returnValue: true)] this ITypeSymbol? typeSymbol) =>
		typeSymbol.IsAsParametersAttribute() || typeSymbol.IsFromXxxAttribute();

	public static bool IsFromXxxAttribute(this ITypeSymbol? typeSymbol) =>
		typeSymbol is INamedTypeSymbol
		{
			Name: "FromBodyAttribute"
				or "FromFormAttribute"
				or "FromHeaderAttribute"
				or "FromQueryAttribute"
				or "FromRouteAttribute",
			ContainingNamespace:
			{
				Name: "Mvc",
				ContainingNamespace:
				{
					Name: "AspNetCore",
					ContainingNamespace:
					{
						Name: "Microsoft",
						ContainingNamespace.IsGlobalNamespace: true,
					},
				},
			},
		};

	public static bool IsAsParametersAttribute(this ITypeSymbol? typeSymbol) =>
		typeSymbol is INamedTypeSymbol
		{
			Arity: 0,
			Name: "AsParametersAttribute",
			ContainingNamespace.IsMicrosoftAspNetCoreHttp: true,
		};

	public static bool IsIFormFile(this ITypeSymbol? typeSymbol) =>
		typeSymbol is INamedTypeSymbol
		{
			Arity: 0,
			Name: "IFormFile",
			ContainingNamespace.IsMicrosoftAspNetCoreHttp: true,
		};

	extension(INamespaceSymbol namespaceSymbol)
	{
		public bool IsImmediateApisShared =>
			namespaceSymbol is
			{
				Name: "Shared",
				ContainingNamespace:
				{
					Name: "Apis",
					ContainingNamespace:
					{
						Name: "Immediate",
						ContainingNamespace.IsGlobalNamespace: true,
					},
				},
			};

		public bool IsMicrosoftAspNetCoreAuthorization =>
			namespaceSymbol is
			{
				Name: "Authorization",
				ContainingNamespace:
				{
					Name: "AspNetCore",
					ContainingNamespace:
					{
						Name: "Microsoft",
						ContainingNamespace.IsGlobalNamespace: true,
					},
				},
			};

		public bool IsMicrosoftAspNetCoreBuilder =>
			namespaceSymbol is
			{
				Name: "Builder",
				ContainingNamespace:
				{
					Name: "AspNetCore",
					ContainingNamespace:
					{
						Name: "Microsoft",
						ContainingNamespace.IsGlobalNamespace: true,
					},
				},
			};

		public bool IsMicrosoftAspNetCoreHttp =>
			namespaceSymbol is
			{
				Name: "Http",
				ContainingNamespace:
				{
					Name: "AspNetCore",
					ContainingNamespace:
					{
						Name: "Microsoft",
						ContainingNamespace.IsGlobalNamespace: true,
					},
				},
			};
	}
}
