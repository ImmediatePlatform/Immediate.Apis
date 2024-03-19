using System.Collections.Immutable;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Scriban;

namespace Immediate.Apis.Generators;

internal static class Utility
{
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
