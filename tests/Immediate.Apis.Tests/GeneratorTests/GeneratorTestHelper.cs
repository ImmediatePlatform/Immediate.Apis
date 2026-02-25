using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using Immediate.Apis.Generators;
using Immediate.Handlers.Generators;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Immediate.Apis.Tests.GeneratorTests;

public static class GeneratorTestHelper
{
	public static GeneratorDriverRunResult RunGenerator(
		[StringSyntax("c#-test")] string source,
		LanguageVersion languageVersion = LanguageVersion.CSharp13,
		params ReadOnlySpan<string> skippedSteps
	)
	{
		var options = CSharpParseOptions.Default.WithLanguageVersion(languageVersion);

		var syntaxTree = CSharpSyntaxTree.ParseText(
			source,
			options,
			cancellationToken: TestContext.Current.CancellationToken
		);

		var compilation = CSharpCompilation.Create(
			assemblyName: "Tests",
			syntaxTrees: [syntaxTree],
			references:
			[
				..Utility.NetCoreAssemblies,
				..Utility.GetMetadataReferences(),
			],
			options: new(
				outputKind: OutputKind.DynamicallyLinkedLibrary
			)
		);

		GeneratorDriver driver = CSharpGeneratorDriver.Create(
			generators:
			[
				new ImmediateApisGenerator().AsSourceGenerator(),
				new ImmediateHandlersGenerator().AsSourceGenerator(),
			],
			parseOptions: options,
			driverOptions: new GeneratorDriverOptions(default, trackIncrementalGeneratorSteps: true)
		);

		driver = RunGenerator(driver, compilation);
		var result = driver.GetRunResult();

		VerifyIncrementality(driver, compilation, options, skippedSteps);

		return result;
	}

	private static GeneratorDriver RunGenerator(
		GeneratorDriver driver,
		Compilation compilation
	)
	{
		driver = driver
			.RunGeneratorsAndUpdateCompilation(
				compilation,
				out var outputCompilation,
				out var diagnostics,
				TestContext.Current.CancellationToken
			);

		Assert.Empty(
			outputCompilation
				.GetDiagnostics(TestContext.Current.CancellationToken)
				.Where(d => d.Severity is DiagnosticSeverity.Error or DiagnosticSeverity.Warning)
		);

		Assert.Empty(diagnostics);
		return driver;
	}

	private static void VerifyIncrementality(
		GeneratorDriver driver,
		Compilation compilation,
		CSharpParseOptions options,
		ReadOnlySpan<string> skippedSteps
	)
	{
		var clone = compilation.Clone().AddSyntaxTrees(
			CSharpSyntaxTree.ParseText(
				"// dummy",
				options,
				cancellationToken: TestContext.Current.CancellationToken
			)
		);

		driver = RunGenerator(driver, clone);

		if (
			driver.GetRunResult() is not
			{
				Results:
				[
				{
					TrackedOutputSteps: { } outputSteps,
					TrackedSteps: { } trackedSteps,
				},
					_
				],
			}
		)
		{
			Assert.Fail("Unable to verify incrementality.");
			return;
		}

		foreach (var (_, step) in outputSteps)
			AssertSteps(step);

		foreach (var step in TrackedSteps)
		{
			if (skippedSteps.Contains(step))
			{
				if (trackedSteps.ContainsKey(step))
					Assert.Fail($"Step `{step}` should have been skipped, but is present.");
			}
			else
			{
				if (!trackedSteps.TryGetValue(step, out var outputs))
					Assert.Fail($"Step `{step}` expected, but is missing.");

				AssertSteps(outputs);
			}
		}
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
