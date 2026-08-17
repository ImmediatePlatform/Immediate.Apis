using System.Runtime.CompilerServices;
using Xunit.Sdk;
using Xunit.v3;

[assembly: Parallelization(Mode = ParallelMode.All)]

namespace Immediate.Apis.Tests;

public static class ModuleInitializer
{
	[ModuleInitializer]
	public static void Init()
	{
		VerifierSettings.AutoVerify(includeBuildServer: false);
		VerifierSettings.ScrubLinesContaining("cs", comparison: StringComparison.Ordinal, "GeneratedCodeAttribute");
		UseSourceFileRelativeDirectory("Snapshots");

		VerifySourceGenerators.Initialize();
	}
}
