using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis;

namespace Immediate.Apis.Generators;

internal static class ITypeSymbolExtensions
{
	public static bool IsMapMethodAttribute(this ITypeSymbol? typeSymbol) =>
		typeSymbol is INamedTypeSymbol
		{
			ContainingNamespace:
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
			},
			Name: "MapMethodAttribute",
		}
		|| (typeSymbol?.BaseType is { } bt && IsMapMethodAttribute(bt));

	public static bool IsMicrosoftAspNetCoreAuthorization(this INamespaceSymbol symbol) =>
		symbol is
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

	public static bool IsAllowAnonymous(this ITypeSymbol? typeSymbol) =>
		typeSymbol?.Name is "AllowAnonymousAttribute"
		&& typeSymbol.ContainingNamespace.IsMicrosoftAspNetCoreAuthorization();

	public static bool IsAuthorize(this ITypeSymbol? typeSymbol) =>
		typeSymbol?.Name is "AuthorizeAttribute"
		&& typeSymbol.ContainingNamespace.IsMicrosoftAspNetCoreAuthorization();

	public static bool IsIEndpointConventionBuilderOrRouteHandlerBuilder(this ITypeSymbol? typeSymbol) =>
		typeSymbol is
		{
			Name: "IEndpointConventionBuilder" or "RouteHandlerBuilder",
			ContainingNamespace:
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
			},
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

	public static bool IsAsParametersAttribute(this ITypeSymbol? typeSymbol) =>
		typeSymbol is INamedTypeSymbol
		{
			Name: "AsParametersAttribute",
			ContainingNamespace:
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
			},
		};

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

	public static bool IsIFormFile(this ITypeSymbol? typeSymbol) =>
		typeSymbol is INamedTypeSymbol
		{
			Name: "IFormFile",
			ContainingNamespace:
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
			},
		};
}
