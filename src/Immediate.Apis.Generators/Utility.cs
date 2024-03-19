using System.Collections.Immutable;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Scriban;

namespace Immediate.Apis.Generators;

internal static class Utility
{
	public static bool ImplementsBaseClass(this INamedTypeSymbol typeSymbol, INamedTypeSymbol typeToCheck) =>
		SymbolEqualityComparer.Default.Equals(typeSymbol, typeToCheck)
		|| (typeSymbol.BaseType is not null
			&& ImplementsBaseClass(typeSymbol.BaseType.OriginalDefinition, typeToCheck)
		   );

	public static ITypeSymbol? GetTaskReturnType(this IMethodSymbol method, INamedTypeSymbol taskSymbol) =>
		SymbolEqualityComparer.Default.Equals(method.ReturnType.OriginalDefinition, taskSymbol)
			? ((INamedTypeSymbol)method.ReturnType).TypeArguments.FirstOrDefault()
			: null;

	public static AttributeData? GetAttribute(this INamedTypeSymbol symbol, string attribute) =>
		symbol
			.GetAttributes()
			.FirstOrDefault(a =>
				a.AttributeClass?.ToString() == attribute
			);

	public static string? NullIf(this string value, string check) =>
		value.Equals(check, StringComparison.Ordinal) ? null : value;

	public static Template GetTemplate(string name)
	{
		using var stream = Assembly
			.GetExecutingAssembly()
			.GetManifestResourceStream(
				$"Immediate.Apis.Generators.Templates.{name}.sbntxt"
			)!;

		using var reader = new StreamReader(stream);
		return Template.Parse(reader.ReadToEnd());
	}

	public static IncrementalValueProvider<EquatableReadOnlyList<T>> Concat<T>(
		this IncrementalValueProvider<ImmutableArray<T>> method1,
		IncrementalValueProvider<ImmutableArray<T>> method2,
		IncrementalValueProvider<ImmutableArray<T>> method3,
		IncrementalValueProvider<ImmutableArray<T>> method4,
		IncrementalValueProvider<ImmutableArray<T>> method5
	) => method1.Combine(method2)
			.Combine(method3.Combine(method4))
			.Combine(method5)
			.Select((x, _) =>
				new EquatableReadOnlyList<T>([
					.. x.Left.Left.Left,
					.. x.Left.Left.Right,
					.. x.Left.Right.Left,
					.. x.Left.Right.Right,
					.. x.Right,
				]));
}
