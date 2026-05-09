using Microsoft.CodeAnalysis.CSharp;

namespace Immediate.Apis;

internal static class RouteGroupUtility
{
	public static bool IsValidRouteGroupName(string? value)
	{
		if (string.IsNullOrEmpty(value))
		{
			return false;
		}

		for (var i = 0; i < value!.Length; i++)
		{
			if (!SyntaxFacts.IsIdentifierPartCharacter(value[i]))
			{
				return false;
			}
		}

		return true;
	}
}
