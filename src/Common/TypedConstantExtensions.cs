using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Immediate.Apis;

internal static class TypedConstantExtensions
{
	public static TypedConstant? GetArgumentValue(this ImmutableArray<KeyValuePair<string, TypedConstant>> arguments, string name)
	{
		foreach (var argument in arguments)
		{
			if (string.Equals(name, argument.Key, StringComparison.Ordinal))
				return argument.Value;
		}

		return null;
	}

	public static string? GetStringArray(this ImmutableArray<KeyValuePair<string, TypedConstant>> arguments, string name) =>
		arguments.GetArgumentValue(name)?.GetStringArray();

	extension(TypedConstant constant)
	{
		public string? GetStringArray()
		{
			if (constant.Kind != TypedConstantKind.Array)
				return null;

			return string.Join(
				", ",
				constant.Values
					.Select(tc => tc.ToCSharpString())
					.OrderBy(x => x, StringComparer.Ordinal)
			);
		}
	}
}
