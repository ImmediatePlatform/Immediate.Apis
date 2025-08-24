using System.Collections.Immutable;
using Immediate.Apis.Generators;
using Immediate.Handlers.Generators;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Immediate.Apis.Tests.GeneratorTests;

public static class GeneratorTestHelper
{
	public static GeneratorDriverRunResult RunGenerator(string source)
	{
		var syntaxTree = CSharpSyntaxTree.ParseText(source);

		var compilation = CSharpCompilation.Create(
			assemblyName: "Tests",
			syntaxTrees: [syntaxTree],
			references:
			[
				.. Basic.Reference.Assemblies.AspNet80.References.All,
				.. Utility.GetMetadataReferences(),
			],
			options: new(
				outputKind: OutputKind.DynamicallyLinkedLibrary
			)
		);

		var clone = compilation.Clone().AddSyntaxTrees(CSharpSyntaxTree.ParseText("// dummy"));

		GeneratorDriver driver = CSharpGeneratorDriver.Create(
			generators:
			[
				new ImmediateApisGenerator().AsSourceGenerator(),
				new ImmediateHandlersGenerator().AsSourceGenerator(),
			],
			driverOptions: new GeneratorDriverOptions(default, trackIncrementalGeneratorSteps: true)
		);

		var result1 = RunGenerator(ref driver, compilation);
		var result2 = RunGenerator(ref driver, clone);

		foreach (var (_, step) in result2.Results[0].TrackedOutputSteps)
			AssertSteps(step);

		foreach (var step in TrackedSteps)
		{
			if (result2.Results[0].TrackedSteps.TryGetValue(step, out var outputs))
				AssertSteps(outputs);
		}

		return result1;
	}

	private static GeneratorDriverRunResult RunGenerator(
		ref GeneratorDriver driver,
		Compilation compilation
	)
	{
		driver = driver
			.RunGeneratorsAndUpdateCompilation(
				compilation,
				out var outputCompilation,
				out var diagnostics
			);

		Assert.Empty(
			outputCompilation
				.GetDiagnostics()
				.Where(d => d.Severity is DiagnosticSeverity.Error or DiagnosticSeverity.Warning)
		);

		Assert.Empty(diagnostics);
		return driver.GetRunResult();
	}

	private static ReadOnlySpan<string> TrackedSteps =>
		new string[]
		{
			"AssemblyName",
			"Handlers",
		};

	private static void AssertSteps(
		ImmutableArray<IncrementalGeneratorRunStep> steps
	)
	{
		var outputs = steps.SelectMany(o => o.Outputs);

		Assert.All(outputs, o => Assert.True(o.Reason is IncrementalStepRunReason.Unchanged or IncrementalStepRunReason.Cached));
	}
}
