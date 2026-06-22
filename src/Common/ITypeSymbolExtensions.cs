using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis;

namespace Immediate.Apis;

internal static class ITypeSymbolExtensions
{
	extension([NotNullWhen(true)] ITypeSymbol? typeSymbol)
	{

		public bool IsMapMethodAttribute =>
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

		public bool IsAllowAnonymousAttribute =>
			typeSymbol is INamedTypeSymbol
			{
				Arity: 0,
				Name: "AllowAnonymousAttribute",
				ContainingNamespace.IsMicrosoftAspNetCoreAuthorization: true,
			};

		public bool IsAuthorizeAttribute =>
			typeSymbol is INamedTypeSymbol
			{
				Arity: 0,
				Name: "AuthorizeAttribute",
				ContainingNamespace.IsMicrosoftAspNetCoreAuthorization: true,
			};

		public bool IsHandlerAttribute =>
			typeSymbol is
			{
				Name: "HandlerAttribute",
				ContainingNamespace.IsImmediateHandlersShared: true,
			};

		public bool IsIEndpointConventionBuilderOrRouteHandlerBuilder =>
			typeSymbol is INamedTypeSymbol
			{
				Arity: 0,
				Name: "IEndpointConventionBuilder" or "RouteHandlerBuilder",
				ContainingNamespace.IsMicrosoftAspNetCoreBuilder: true,
			};

		public bool IsValueTask1 =>
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

		public bool IsBindingParameterAttribute =>
			typeSymbol.IsAsParametersAttribute || typeSymbol.IsFromXxxAttribute;

		public bool IsFromXxxAttribute =>
			typeSymbol is INamedTypeSymbol
			{
				Name: "FromBodyAttribute"
					or "FromFormAttribute"
					or "FromHeaderAttribute"
					or "FromQueryAttribute"
					or "FromRouteAttribute",
				ContainingNamespace.IsMicrosoftAspNetCoreMvc: true,
			};

		public bool IsAsParametersAttribute =>
			typeSymbol is INamedTypeSymbol
			{
				Arity: 0,
				Name: "AsParametersAttribute",
				ContainingNamespace.IsMicrosoftAspNetCoreHttp: true,
			};

		public bool IsIFormFile =>
			typeSymbol is INamedTypeSymbol
			{
				Arity: 0,
				Name: "IFormFile",
				ContainingNamespace.IsMicrosoftAspNetCoreHttp: true,
			};

		public bool IsRouteGroupAttribute =>
			typeSymbol is INamedTypeSymbol
			{
				Arity: 0,
				Name: "RouteGroupAttribute",
				ContainingNamespace.IsImmediateApisShared: true,
			};

		public bool IsImmediateAssemblyIdentifierAttribute =>
			typeSymbol is INamedTypeSymbol
			{
				Arity: 0,
				Name: "ImmediateAssemblyIdentifierAttribute",
				ContainingNamespace.IsImmediateHandlersShared: true,
			};
	}

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

		public bool IsImmediateHandlersShared =>
			namespaceSymbol is
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

		public bool IsMicrosoftAspNetCoreMvc =>
			namespaceSymbol is
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
			};
	}
}
