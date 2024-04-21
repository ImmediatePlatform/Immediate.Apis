using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Microsoft.AspNetCore.Http;

[ExcludeFromCodeCoverage]
public static class TypedResults
{
	public static Ok<TValue> Ok<TValue>(TValue result)
	{
		return new Ok<TValue>();
	}
}
