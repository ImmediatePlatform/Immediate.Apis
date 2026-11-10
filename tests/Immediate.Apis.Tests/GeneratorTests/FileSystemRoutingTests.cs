using System.Collections.Immutable;
using System.Reflection;
using Immediate.Apis.Analyzers;
using Immediate.Apis.Generators;
using Immediate.Handlers.Generators;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.Metadata;
using Microsoft.AspNetCore.Routing;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.Extensions.DependencyInjection;

namespace Immediate.Apis.Tests.GeneratorTests;

public sealed class FileSystemRoutingTests
{
	private static readonly string[] s_unrelatedTags = ["unrelated"];

	[Theory]
	[InlineData("PascalCase", "WeatherForecast")]
	[InlineData("CamelCase", "weatherForecast")]
	[InlineData("LowerCase", "weatherforecast")]
	[InlineData("KebabCase", "weather-forecast")]
	[InlineData("SnakeCase", "weather_forecast")]
	public void DirectoryNamesUseTheConfiguredPolicy(string policy, string segment)
	{
		var (compilation, result) = Generate(policy, ("/project/Api/WeatherForecast/Create.cs", Handler("Create")));
		AssertCompiles(compilation);
		Assert.Contains($"MapGroup(app, \"{segment}\")", Sources(result), StringComparison.Ordinal);
		Assert.Matches("MapPost\\(\\s*app,\\s*\"\"", Sources(result));
	}

	[Fact]
	public void SeparateGroupFilesCustomizeTheDirectoryHierarchy()
	{
		var (compilation, result) = Generate("LowerCase",
			("/project/Api/Root.cs", """
			using Immediate.Apis.Shared;
			using Microsoft.AspNetCore.Routing;
			namespace Example;
			[RouteGroup("api")]
			public partial class Root
			{
				private static void CustomizeGroup(RouteGroupBuilder group) { }
			}
			"""),
			("/project/Api/Public/PublicGroup.cs", """
			using Immediate.Apis.Shared;
			namespace Separate;
			[RouteGroup("public")]
			public partial class PublicGroup;
			"""),
			("/project/Api/Public/Products/Create.cs", Handler("Create")));
		AssertCompiles(compilation);
		var sources = Sources(result);
		Assert.Contains("global::Separate.PublicGroup.MapGroup(group, tags);", sources, StringComparison.Ordinal);
		Assert.Contains("CustomizeGroup(group);", sources, StringComparison.Ordinal);
		Assert.Contains("= \"api/public/products/\";", sources, StringComparison.Ordinal);
	}

	[Fact]
	public void ExplicitRoutesAndGroupsRemainAuthoritative()
	{
		var (compilation, result) = Generate("LowerCase",
			("/outside/Explicit.cs", Handler("Explicit", "\"fixed\"")),
			("/outside/Grouped.cs", Handler("Grouped", group: "[MapGroup<Example.Manual>]")),
			("/outside/Manual.cs", "using Immediate.Apis.Shared; namespace Example; [RouteGroup(\"manual\")] public partial class Manual;"));
		AssertCompiles(compilation);
		Assert.DoesNotContain("FileSystemDirectory_", Sources(result), StringComparison.Ordinal);
		Assert.Contains("global::Example.Grouped.MapEndpoint(group);", Sources(result), StringComparison.Ordinal);
	}

	[Theory]
	[InlineData("/project/Api/Create.cs", "/project/")]
	[InlineData("C:\\project\\Api\\Create.cs", "C:\\project\\")]
	[InlineData("\\\\server\\share\\project\\Api\\Create.cs", "//server/share/project/")]
	public void RootDirectoryAndBothPathSeparatorsAreSupported(string path, string projectDirectory)
	{
		ArgumentNullException.ThrowIfNull(path);
		ArgumentNullException.ThrowIfNull(projectDirectory);
		var (compilation, result) = Generate("LowerCase", [(path, Handler("Create"))], projectDirectory: projectDirectory);
		AssertCompiles(compilation);
		Assert.Contains("global::Example.Create.MapEndpoint(group);", Sources(result), StringComparison.Ordinal);
		Assert.DoesNotContain("FileSystemDirectory_", Sources(result), StringComparison.Ordinal);
	}

	[Theory]
	[InlineData("/outside/Create.cs")]
	[InlineData("/project/ApiSibling/Create.cs")]
	[InlineData("")]
	public async Task MissingOrOutsideSourcePathsAreDiagnosed(string path)
	{
		var (compilation, _) = Generate("LowerCase", (path, Handler("Create")));
		Assert.Contains(await Diagnostics(compilation), d => string.Equals(d.Id, "IAPI0015", StringComparison.Ordinal));
	}

	[Fact]
	public async Task PartialDeclarationsInDifferentDirectoriesAreDiagnosed()
	{
		var (compilation, _) = Generate("LowerCase",
			("/project/Api/Products/Create.cs", Handler("Create")),
			("/project/Api/Other/Create.cs", "namespace Example; public static partial class Create;"));
		Assert.Contains(await Diagnostics(compilation), d => string.Equals(d.Id, "IAPI0015", StringComparison.Ordinal));
	}

	[Fact]
	public async Task AmbiguousGroupsAndNamingCollisionsAreDiagnosed()
	{
		var (groups, _) = Generate("LowerCase",
			("/project/Api/Products/Create.cs", Handler("Create")),
			("/project/Api/Products/Groups.cs", "using Immediate.Apis.Shared; namespace Example; [RouteGroup(\"one\")] public partial class One; [RouteGroup(\"two\")] public partial class Two;"));
		Assert.Contains(await Diagnostics(groups), d => string.Equals(d.Id, "IAPI0015", StringComparison.Ordinal));
		var (collision, _) = Generate("LowerCase",
			("/project/Api/Products/Create.cs", Handler("Create")),
			("/project/Api/PRODUCTS/Other.cs", Handler("Other")));
		Assert.Contains(await Diagnostics(collision), d => string.Equals(d.Id, "IAPI0015", StringComparison.Ordinal));
	}

	[Fact]
	public async Task DuplicateInferredAndExplicitRoutesAreDiagnosed()
	{
		var (compilation, _) = Generate("LowerCase",
			("/project/Api/Products/Create.cs", Handler("Create")),
			("/project/Api/Products/Other.cs", Handler("Other")),
			("/project/Explicit.cs", Handler("Explicit", "\"products\"")));
		Assert.Equal(3, (await Diagnostics(compilation)).Count(d => string.Equals(d.Id, "IAPI0009", StringComparison.Ordinal)));
	}

	[Fact]
	public void FileMovesChangeRoutesAndUnrelatedEditsReuseOutputs()
	{
		var options = CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.Latest);
		var tree = CSharpSyntaxTree.ParseText(Handler("Create"), options, "/project/Api/Products/Create.cs", cancellationToken: TestContext.Current.CancellationToken);
		var input = Input("LowerCase", [tree], options);
		var driver = Driver(options).RunGenerators(input, TestContext.Current.CancellationToken);
		var first = Sources(driver.GetRunResult());
		driver = driver.RunGenerators(input.AddSyntaxTrees(CSharpSyntaxTree.ParseText("// unrelated", options, cancellationToken: TestContext.Current.CancellationToken)), TestContext.Current.CancellationToken);
		Assert.Equal(first, Sources(driver.GetRunResult()));
		Assert.All(driver.GetRunResult().Results[0].TrackedOutputSteps.SelectMany(s => s.Value).SelectMany(s => s.Outputs),
			o => Assert.True(o.Reason is IncrementalStepRunReason.Cached or IncrementalStepRunReason.Unchanged));
		driver = driver.RunGenerators(input.ReplaceSyntaxTree(tree, tree.WithFilePath("/project/Api/Users/Create.cs")), TestContext.Current.CancellationToken);
		Assert.Contains("MapGroup(app, \"users\")", Sources(driver.GetRunResult()), StringComparison.Ordinal);
		Assert.DoesNotContain("MapGroup(app, \"products\")", Sources(driver.GetRunResult()), StringComparison.Ordinal);
	}

	[Fact]
	public void RuntimeGroupsPreservePrefixesTagsAndAuthorization()
	{
		var options = CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.CSharp12);
		var input = Input("LowerCase",
		[
			CSharpSyntaxTree.ParseText(Handler("Create"), options, "/project/Api/Products/Create.cs", cancellationToken: TestContext.Current.CancellationToken),
			CSharpSyntaxTree.ParseText("""
				using Immediate.Apis.Shared;
				using Microsoft.AspNetCore.Builder;
				using Microsoft.AspNetCore.Http;
				using Microsoft.AspNetCore.Routing;
				namespace Example;
				[RouteGroup("api")]
				public partial class Root
				{
					private static void CustomizeGroup(RouteGroupBuilder group) => group.WithTags("root");
				}
				""", options, "/project/Api/Root.cs", cancellationToken: TestContext.Current.CancellationToken),
			CSharpSyntaxTree.ParseText("""
				using Immediate.Apis.Shared;
				using Microsoft.AspNetCore.Builder;
				using Microsoft.AspNetCore.Routing;
				namespace Other;
				[RouteGroup("items", Tags = new[] { "products" })]
				public partial class Products
				{
					private static void CustomizeGroup(RouteGroupBuilder group) => group.RequireAuthorization("products-policy");
				}
				""", options, "/project/Api/Products/Group.cs", cancellationToken: TestContext.Current.CancellationToken),
		], options);
		_ = Driver(options).RunGeneratorsAndUpdateCompilation(input, out var output, out var diagnostics, TestContext.Current.CancellationToken);
		Assert.Empty(diagnostics);
		using var stream = new MemoryStream();
		var emitted = output.Emit(stream, cancellationToken: TestContext.Current.CancellationToken);
		Assert.True(emitted.Success, string.Join("\n", emitted.Diagnostics));
		var assembly = Assembly.Load(stream.ToArray());
		var map = assembly.GetType("ApisServiceCollectionExtensions")!.GetMethod("MapFileSystemTestsEndpoints")!;
		using var provider = new ServiceCollection().AddLogging().AddRouting().BuildServiceProvider();
		var builder = new EndpointBuilder(provider);
		_ = map.Invoke(null, [builder, "/v1", Array.Empty<string>()]);
		var endpoint = Assert.Single(builder.DataSources.SelectMany(s => s.Endpoints).OfType<RouteEndpoint>());
		Assert.Equal("/v1/api/items/", endpoint.RoutePattern.RawText);
		Assert.Contains("root", endpoint.Metadata.GetMetadata<ITagsMetadata>()!.Tags);
		Assert.Equal("products-policy", endpoint.Metadata.GetMetadata<IAuthorizeData>()!.Policy);
		var excluded = new EndpointBuilder(provider);
		_ = map.Invoke(null, [excluded, "/v1", s_unrelatedTags]);
		Assert.Empty(excluded.DataSources.SelectMany(s => s.Endpoints));
	}

	[Fact]
	public async Task InvalidConfigurationAndEmptySegmentsAreDiagnosed()
	{
		var (outside, _) = Generate("LowerCase", [("/project/Api/Create.cs", Handler("Create"))], projectDirectory: "");
		Assert.Contains(await Diagnostics(outside, ""), d => string.Equals(d.Id, "IAPI0015", StringComparison.Ordinal));
		var (empty, _) = Generate("LowerCase", ("/project/Api/!!!/Create.cs", Handler("Create")));
		Assert.Contains(await Diagnostics(empty), d => string.Equals(d.Id, "IAPI0015", StringComparison.Ordinal));
		var options = CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.Latest);
		var input = Input("LowerCase", [CSharpSyntaxTree.ParseText(Handler("Create"), options, "/project/Api/Create.cs", cancellationToken: TestContext.Current.CancellationToken)], options);
		var assembly = input.SyntaxTrees.First();
		foreach (var invalid in new[] { "../Api", "/absolute" })
		{
			var invalidRoot = input.ReplaceSyntaxTree(assembly, CSharpSyntaxTree.ParseText(assembly.ToString().Replace("RootDirectory = \"Api\"", $"RootDirectory = \"{invalid}\"", StringComparison.Ordinal), options, "/project/AssemblyInfo.cs", cancellationToken: TestContext.Current.CancellationToken));
			Assert.Contains(await Diagnostics(invalidRoot), d => string.Equals(d.Id, "IAPI0015", StringComparison.Ordinal));
		}
	}

	[Fact]
	public void RoutingRemainsOptIn()
	{
		var options = CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.Latest);
		var input = Input("LowerCase", [CSharpSyntaxTree.ParseText(Handler("Create"), options, "/project/Api/Products/Create.cs", cancellationToken: TestContext.Current.CancellationToken)], options);
		input = input.RemoveSyntaxTrees(input.SyntaxTrees.First());
		var result = Driver(options).RunGenerators(input, TestContext.Current.CancellationToken).GetRunResult();
		Assert.DoesNotContain("MapPost(", Sources(result), StringComparison.Ordinal);
	}

	[Fact]
	public async Task DefaultConfigurationUsesProjectRelativeLowercaseDirectories()
	{
		var options = CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.Latest);
		var input = Input("LowerCase", [CSharpSyntaxTree.ParseText(Handler("Create"), options, "/project/Api/Products/Create.cs", cancellationToken: TestContext.Current.CancellationToken)], options);
		input = input.ReplaceSyntaxTree(input.SyntaxTrees.First(), CSharpSyntaxTree.ParseText("using Immediate.Apis.Shared; [assembly: FileSystemRouting]", options, "/project/AssemblyInfo.cs", cancellationToken: TestContext.Current.CancellationToken));
		var driver = Driver(options).RunGeneratorsAndUpdateCompilation(input, out var output, out var diagnostics, TestContext.Current.CancellationToken);
		Assert.Empty(diagnostics);
		AssertCompiles(output);
		Assert.Empty(await Diagnostics(output));
		Assert.Contains("MapGroup(app, \"api\")", Sources(driver.GetRunResult()), StringComparison.Ordinal);
		Assert.Contains("MapGroup(app, \"products\")", Sources(driver.GetRunResult()), StringComparison.Ordinal);
	}

	[Fact]
	public void WindowsPathsMatchGroupsWithoutCaseSensitivity()
	{
		var (compilation, result) = Generate("LowerCase",
		[
			("c:\\PROJECT\\api\\Products\\Create.cs", Handler("Create")),
			("C:\\project\\API\\products\\Other.cs", Handler("Other", "\"other\"")),
			("C:\\project\\Api\\PRODUCTS\\Group.cs", "using Immediate.Apis.Shared; namespace Example; [RouteGroup(\"items\")] public partial class Products;"),
		], projectDirectory: "C:\\project\\");
		AssertCompiles(compilation);
		Assert.Contains("MapGroup(app, \"items\")", Sources(result), StringComparison.Ordinal);
	}

	[Fact]
	public void PunctuationSeparatesWords()
	{
		var (compilation, result) = Generate("KebabCase", ("/project/Api/Weather & Forecast/Create.cs", Handler("Create")));
		AssertCompiles(compilation);
		Assert.Contains("MapGroup(app, \"weather-forecast\")", Sources(result), StringComparison.Ordinal);
	}

	[Theory]
	[InlineData("Other.FileSystemRouting", "namespace Other { public sealed class FileSystemRoutingAttribute : System.Attribute; }")]
	[InlineData("Immediate.Apis.Shared.FileSystemRouting<int>", "namespace Immediate.Apis.Shared { public sealed class FileSystemRoutingAttribute<T> : System.Attribute; }")]
	[InlineData("Immediate.Apis.Shared.Container.FileSystemRouting", "namespace Immediate.Apis.Shared { public static class Container { public sealed class FileSystemRoutingAttribute : System.Attribute; } }")]
	public async Task SimilarlyNamedAttributesDoNotEnableRouting(string attribute, string declaration)
	{
		var options = CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.Latest);
		var input = Input("LowerCase", [CSharpSyntaxTree.ParseText(Handler("Create"), options, "/project/Api/Products/Create.cs", cancellationToken: TestContext.Current.CancellationToken)], options);
		input = input.ReplaceSyntaxTree(input.SyntaxTrees.First(), CSharpSyntaxTree.ParseText($"[assembly: {attribute}]\n{declaration}", options, "/project/AssemblyInfo.cs", cancellationToken: TestContext.Current.CancellationToken));
		var driver = Driver(options).RunGeneratorsAndUpdateCompilation(input, out var output, out var diagnostics, TestContext.Current.CancellationToken);
		Assert.Empty(diagnostics);
		AssertCompiles(output);
		Assert.Empty(await Diagnostics(output));
		Assert.DoesNotContain("MapPost(", Sources(driver.GetRunResult()), StringComparison.Ordinal);
	}

	private sealed class EndpointBuilder(IServiceProvider services) : IEndpointRouteBuilder
	{
		public IServiceProvider ServiceProvider => services;
		public ICollection<EndpointDataSource> DataSources { get; } = [];
		public IApplicationBuilder CreateApplicationBuilder() => new ApplicationBuilder(services);
	}

	private static string Handler(string name, string route = "", string group = "") => $$"""
		using System.Threading;
		using System.Threading.Tasks;
		using Immediate.Apis.Shared;
		using Immediate.Handlers.Shared;
		namespace Example;
		[Handler]
		[MapPost({{route}})]
		{{group}}
		public static partial class {{name}}
		{
			public sealed record Command;
			private static ValueTask<int> Handle(Command _, CancellationToken token) => ValueTask.FromResult(1);
		}
		""";

	private static (Compilation Compilation, GeneratorDriverRunResult Result) Generate(string policy, params (string Path, string Source)[] sources)
		=> Generate(policy, sources, "/project/");

	private static (Compilation Compilation, GeneratorDriverRunResult Result) Generate(string policy, (string Path, string Source)[] sources, string projectDirectory)
	{
		var options = CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.Latest);
		var compilation = Input(policy, sources.Select(s => CSharpSyntaxTree.ParseText(s.Source, options, s.Path)), options);
		var driver = Driver(options, projectDirectory).RunGeneratorsAndUpdateCompilation(compilation, out var output, out var diagnostics);
		Assert.Empty(diagnostics);
		return (output, driver.GetRunResult());
	}

	private static CSharpCompilation Input(string policy, IEnumerable<SyntaxTree> trees, CSharpParseOptions options) => CSharpCompilation.Create(
		"FileSystemTests", trees.Prepend(CSharpSyntaxTree.ParseText($"using Immediate.Apis.Shared; [assembly: FileSystemRouting(RootDirectory = \"Api\", NamingPolicy = RouteNamingPolicy.{policy})]", options, "/project/AssemblyInfo.cs")),
		[.. Utility.NetCoreAssemblies, .. Utility.GetMetadataReferences()], new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

	private static CSharpGeneratorDriver Driver(CSharpParseOptions options, string projectDirectory = "/project/") => CSharpGeneratorDriver.Create(
		[new ImmediateApisGenerator().AsSourceGenerator(), new ImmediateHandlersGenerator().AsSourceGenerator()],
		parseOptions: options, optionsProvider: new OptionsProvider(projectDirectory),
		driverOptions: new GeneratorDriverOptions(default, trackIncrementalGeneratorSteps: true));

	private static string Sources(GeneratorDriverRunResult result) => string.Join("\n", result.Results[0].GeneratedSources.Select(s => s.SourceText.ToString()));
	private static void AssertCompiles(Compilation compilation) => Assert.Empty(compilation.GetDiagnostics().Where(d => d.Severity == DiagnosticSeverity.Error));
	private static Task<ImmutableArray<Diagnostic>> Diagnostics(Compilation compilation, string projectDirectory = "/project/") => compilation.WithAnalyzers(
		[new FileSystemRoutingAnalyzer()], new AnalyzerOptions([], new OptionsProvider(projectDirectory))).GetAnalyzerDiagnosticsAsync(TestContext.Current.CancellationToken);

	private sealed class OptionsProvider(string projectDirectory) : AnalyzerConfigOptionsProvider
	{
		public override AnalyzerConfigOptions GlobalOptions { get; } = new Options(projectDirectory);
		public override AnalyzerConfigOptions GetOptions(SyntaxTree tree) => GlobalOptions;
		public override AnalyzerConfigOptions GetOptions(AdditionalText textFile) => GlobalOptions;
	}

	private sealed class Options(string projectDirectory) : AnalyzerConfigOptions
	{
		public override bool TryGetValue(string key, out string value)
		{
			value = key.Equals("build_property.ProjectDir", StringComparison.OrdinalIgnoreCase) ? projectDirectory : "";
			return value.Length > 0;
		}
	}
}
