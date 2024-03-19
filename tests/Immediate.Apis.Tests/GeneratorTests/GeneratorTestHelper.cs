using Immediate.Apis.Generators;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Immediate.Apis.Tests.GeneratorTests;

public static class GeneratorTestHelper
{
	public static GeneratorDriver GetDriver(string source)
	{
		// Parse the provided string into a C# syntax tree
		var syntaxTree = CSharpSyntaxTree.ParseText(source);

		// Create a Roslyn compilation for the syntax tree.
		var compilation = CSharpCompilation.Create(
			assemblyName: "Tests",
			syntaxTrees: [syntaxTree],
			references:
			[
				.. Basic.Reference.Assemblies.Net80.References.All,
				MetadataReference.CreateFromFile("./Immediate.Handlers.Shared.dll"),
				MetadataReference.CreateFromFile("./Immediate.Apis.Shared.dll"),
				MetadataReference.CreateFromFile("./Microsoft.Extensions.DependencyInjection.dll"),
				MetadataReference.CreateFromFile("./Microsoft.Extensions.DependencyInjection.Abstractions.dll"),
				MetadataReference.CreateFromFile("./Microsoft.AspNetCore.Authorization.dll"),
				MetadataReference.CreateFromFile("./Microsoft.AspNetCore.Metadata.dll"),
				MetadataReference.CreateFromFile("./Microsoft.Extensions.Logging.Abstractions.dll"),
				MetadataReference.CreateFromFile("./Microsoft.Extensions.Options.dll"),
				MetadataReference.CreateFromFile("./Microsoft.Extensions.Primitives.dll"),
			]
		);

		// Create an instance of our incremental source generator
		var generator = new ImmediateApisGenerator();

		// The GeneratorDriver is used to run our generator against a compilation
		GeneratorDriver driver = CSharpGeneratorDriver.Create(generator);

		// Run the source generator!
		return driver.RunGenerators(compilation);
	}
}
