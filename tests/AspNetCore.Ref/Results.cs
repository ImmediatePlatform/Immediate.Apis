using System.Diagnostics.CodeAnalysis;

#nullable disable
namespace Microsoft.AspNetCore.Http.HttpResults;

#pragma warning disable CA1040
public interface IResult;
#pragma warning restore CA1040

public sealed class Results<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.NonPublicMethods)] TResult1, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.NonPublicMethods)] TResult2> :
  IResult
  where TResult1 : IResult
  where TResult2 : IResult
{
	private Results(IResult activeResult) => this.Result = activeResult;

	public IResult Result { get; }

	public static implicit operator Results<TResult1, TResult2>(TResult1 result)
	{
		return new Results<TResult1, TResult2>(result);
	}

	public static implicit operator Results<TResult1, TResult2>(TResult2 result)
	{
		return new Results<TResult1, TResult2>(result);
	}

	public Results<TResult1, TResult2> ToResults()
	{
		throw new NotImplementedException();
	}
}

public sealed class Ok<TValue> : IResult
{
}

public sealed class NotFound : IResult
{
}
