using Immediate.Apis.Generators;
using Immediate.Handlers.Generators.ImmediateHandlers;
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

		var generator = new ImmediateApisGenerator();

		var driver = CSharpGeneratorDriver
			.Create(new ImmediateApisGenerator(), new ImmediateHandlersGenerator())
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
}
