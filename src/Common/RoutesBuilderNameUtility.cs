using Microsoft.CodeAnalysis.CSharp;

namespace Immediate.Apis;

internal static class RoutesBuilderNameUtility
{
	public static bool IsValidRoutesBuilderName(string? value)
	{
		if (string.IsNullOrWhiteSpace(value))
			return false;

		if (!SyntaxFacts.IsIdentifierStartCharacter(value![0]))
			return false;

		for (var i = 1; i < value.Length; i++)
		{
			if (!SyntaxFacts.IsIdentifierPartCharacter(value[i]))
			{
				return false;
			}
		}

		return true;
	}
}
