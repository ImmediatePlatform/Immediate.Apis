using System.Diagnostics;
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
			);

		Debug.Assert(stream is { });

		using var reader = new StreamReader(stream);
		return Template.Parse(reader.ReadToEnd());
	}

	public static string? NullIf(this string value, string check) =>
		value.Equals(check, StringComparison.Ordinal) ? null : value;

	public static IncrementalValuesProvider<T> WhereNotNull<T>(this IncrementalValuesProvider<T?> values)
		where T : class => values.Where(x => x is not null)!;

	public static IncrementalValueProvider<EquatableDictionary<TKey, EquatableReadOnlyList<TValue>>> GroupBy<T, TKey, TValue>(
		this IncrementalValuesProvider<T> values,
		Func<T, TKey> keyFactory,
		Func<T, TValue> valueFactory
	)
		where TKey : notnull
		where TValue : class, IEquatable<TValue>
	{
		return values
			.Collect()
			.Select((l, ct) => l
				.GroupBy(keyFactory, valueFactory)
				.ToDictionary(
					x => x.Key,
					x => x.ToEquatableReadOnlyList()
				)
				.ToEquatableDictionary()
			);
	}

	public static IncrementalValueProvider<EquatableDictionary<TKey, EquatableReadOnlyList<T>>> GroupBy<T, TKey>(
		this IncrementalValuesProvider<T> values,
		Func<T, TKey> keyFactory
	)
		where TKey : notnull
		where T : class, IEquatable<T>
	{
		return values
			.Collect()
			.Select((l, ct) => l
				.GroupBy(keyFactory)
				.ToDictionary(
					x => x.Key,
					x => x.ToEquatableReadOnlyList()
				)
				.ToEquatableDictionary()
			);
	}
}
