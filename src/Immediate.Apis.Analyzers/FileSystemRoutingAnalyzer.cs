using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Immediate.Apis.Analyzers;

/// <summary>Validates inferred routes and checks their effective paths for duplicates.</summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class FileSystemRoutingAnalyzer : DiagnosticAnalyzer
{
	/// <summary>An invalid or ambiguous source layout prevents directory routing.</summary>
	public static readonly DiagnosticDescriptor InvalidFileSystemRoute = new(
		DiagnosticIds.IAPI0015InvalidFileSystemRoute, "Invalid file system route", "Cannot infer route: {0}", "ImmediateApis",
		DiagnosticSeverity.Error, isEnabledByDefault: true, customTags: [WellKnownDiagnosticTags.CompilationEnd]);

	/// <inheritdoc />
	public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
		ImmutableArray.Create(
		[
			InvalidFileSystemRoute,
			DuplicateEndpointAnalyzer.EndpointHasBeenSpecifiedMultipleTimes,
		]);

	/// <inheritdoc />
	public override void Initialize(AnalysisContext context)
	{
		ArgumentNullException.ThrowIfNull(context);
		context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
		context.EnableConcurrentExecution();
		context.RegisterCompilationStartAction(start =>
		{
			var settings = FileSystemRoutingSettings.Create(start.Compilation, start.Options.AnalyzerConfigOptionsProvider.GlobalOptions);
			if (!settings.Enabled) return;
			var symbols = new List<INamedTypeSymbol>();
			var gate = new Lock();
			start.RegisterSymbolAction(c =>
			{
				lock (gate) symbols.Add((INamedTypeSymbol)c.Symbol);
			}, SymbolKind.NamedType);
			start.RegisterCompilationEndAction(end => Analyze(end, settings, symbols));
		});
	}

	private static void Analyze(CompilationAnalysisContext context, FileSystemRoutingSettings settings, List<INamedTypeSymbol> symbols)
	{
		if (settings.Error is { } configurationError)
		{
			var attribute = context.Compilation.Assembly.GetAttributes().First(a => a.AttributeClass.IsFileSystemRoutingAttribute);
			Report(attribute, configurationError);
			return;
		}

		var declarations = symbols.Select(s => (Symbol: s, Attribute: s.GetAttributes().GetRouteGroupAttribute()))
			.Where(p => p.Attribute is not null)
			.Select(p => new FileSystemGroup
			{
				ClassName = p.Symbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
				ParentClassName = p.Symbol.ContainingType?.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
				FilePath = p.Attribute!.ApplicationSyntaxReference?.SyntaxTree.FilePath ?? "",
				Route = p.Attribute.ConstructorArguments.FirstOrDefault().Value as string ?? "",
			})
			.ToArray();
		var resolver = new FileSystemRouting(settings, declarations);
		var endpoints = new List<Endpoint>();
		foreach (var symbol in symbols.OrderBy(s => s.ToDisplayString(), StringComparer.Ordinal))
		{
			context.CancellationToken.ThrowIfCancellationRequested();
			var attributes = symbol.GetAttributes();
			if (attributes.GetMethodAttribute() is not { } attribute || attribute.GetHttpMethod() is not { } method) continue;
			var routes = attribute.GetRoutes();
			var inferred = routes.Count == 0;
			var group = attributes.GetMapGroupAttribute()?.AttributeClass?.TypeArguments.FirstOrDefault()
				?.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
			if (inferred)
			{
				if (group is null)
				{
					var result = resolver.Resolve(attribute.ApplicationSyntaxReference?.SyntaxTree.FilePath ?? "",
						FileSystemRouting.GetPartialFiles(symbol, context.CancellationToken));
					if (result.Error is { } error) { Report(attribute, error); continue; }

					group = result.GroupClassName;
				}

				routes = [""];
			}

			endpoints.AddRange(routes.Select(route => new Endpoint
			{
				Method = method.ToUpperInvariant(),
				Route = route,
				Group = group,
				Name = symbol.Name,
				Attribute = attribute,
				Inferred = inferred,
			}));
		}

		foreach (var duplicates in endpoints.GroupBy(e => (e.Method,
			Path: FileSystemRouting.Join(resolver.GetGroupPrefix(e.Group), e.Route).ToUpperInvariant()))
			.Where(g => g.Skip(1).Any() && g.Any(e => e.Inferred)))
		{
			foreach (var endpoint in duplicates)
			{
				context.ReportDiagnostic(Diagnostic.Create(DuplicateEndpointAnalyzer.EndpointHasBeenSpecifiedMultipleTimes,
					endpoint.Attribute.ApplicationSyntaxReference?.GetSyntax(context.CancellationToken).GetLocation(),
					endpoint.Method, duplicates.Key.Path, " ", string.Join(", ", duplicates.Select(e => e.Name))));
			}
		}

		void Report(AttributeData attribute, string error) => context.ReportDiagnostic(Diagnostic.Create(
			InvalidFileSystemRoute, attribute.ApplicationSyntaxReference?.GetSyntax(context.CancellationToken).GetLocation(), error));
	}

	private sealed record Endpoint
	{
		public required string Method { get; init; }
		public required string Route { get; init; }
		public required string? Group { get; init; }
		public required string Name { get; init; }
		public required AttributeData Attribute { get; init; }
		public required bool Inferred { get; init; }
	}
}
