using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Testing;

namespace Immediate.Apis.Tests.AnalyzerTests;

public static class AnalyzerTestHelpers
{
	public static CSharpAnalyzerTest<TAnalyzer, DefaultVerifier> CreateAnalyzerTest<TAnalyzer>(
		[StringSyntax("c#-test")] string inputSource
	)
		where TAnalyzer : DiagnosticAnalyzer, new()
	{
		var csTest = new CSharpAnalyzerTest<TAnalyzer, DefaultVerifier>
		{
			TestState =
			{
				Sources = { inputSource },
				ReferenceAssemblies = ReferenceAssemblies.Net.Net80,
			},
		};

		csTest.TestState.AdditionalReferences
			.AddRange(Basic.Reference.Assemblies.AspNet80.References.All);

		csTest.TestState.AdditionalReferences
			.AddRange(Utility.GetMetadataReferences());

		return csTest;
	}

	public static CSharpAnalyzerTest<TAnalyzer, DefaultVerifier> CreateAnalyzerTest<TAnalyzer, TGenerator>(
		[StringSyntax("c#-test")] string inputSource
	)
		where TAnalyzer : DiagnosticAnalyzer, new()
		where TGenerator : IIncrementalGenerator, new()
	{
		var csTest = new CSharpGeneratorAnalyzerTest<TAnalyzer, TGenerator, DefaultVerifier>
		{
			TestBehaviors = TestBehaviors.SkipGeneratedSourcesCheck,
			TestState =
			{
				Sources = { inputSource },
				ReferenceAssemblies = ReferenceAssemblies.Net.Net80,
			},
		};

		csTest.TestState.AdditionalReferences
			.AddRange(Basic.Reference.Assemblies.AspNet80.References.All);

		csTest.TestState.AdditionalReferences
			.AddRange(Utility.GetMetadataReferences());

		return csTest;
	}

	public sealed class CSharpGeneratorAnalyzerTest<TAnalyzer, TGenerator, TVerifier> : CSharpAnalyzerTest<TAnalyzer, TVerifier>
		where TAnalyzer : DiagnosticAnalyzer, new()
		where TGenerator : IIncrementalGenerator, new()
		where TVerifier : IVerifier, new()
	{
		protected override IEnumerable<Type> GetSourceGenerators() =>
			[typeof(TGenerator)];
	}
}
