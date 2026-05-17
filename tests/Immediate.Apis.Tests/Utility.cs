using System.Reflection;
using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;

namespace Immediate.Apis.Tests;

internal static class Utility
{
#if NET8_0
	public static ReferenceAssemblies ReferenceAssemblies => ReferenceAssemblies.Net.Net80;
	public static IEnumerable<MetadataReference> NetCoreAssemblies => Basic.Reference.Assemblies.Net80.References.All;
	private static IEnumerable<MetadataReference> AspNetCoreAssemblies => Basic.Reference.Assemblies.AspNet80.References.All;
#elif NET9_0
	public static ReferenceAssemblies ReferenceAssemblies => ReferenceAssemblies.Net.Net90;
	public static IEnumerable<MetadataReference> NetCoreAssemblies => Basic.Reference.Assemblies.Net90.References.All;
	private static IEnumerable<MetadataReference> AspNetCoreAssemblies => Basic.Reference.Assemblies.AspNet90.References.All;
#elif NET10_0
	public static ReferenceAssemblies ReferenceAssemblies => ReferenceAssemblies.Net.Net100;
	public static IEnumerable<MetadataReference> NetCoreAssemblies => Basic.Reference.Assemblies.Net100.References.All;
	private static IEnumerable<MetadataReference> AspNetCoreAssemblies => Basic.Reference.Assemblies.AspNet100.References.All;
#elif NET11_0
	public static ReferenceAssemblies ReferenceAssemblies { get; } = new ReferenceAssemblies(
		"net11.0",
		new PackageIdentity(
			"Microsoft.NETCore.App.Ref",
			"11.0.0-preview.4.26230.115"
		),
		Path.Combine("ref", "net11.0")
	);
	public static IEnumerable<MetadataReference> NetCoreAssemblies => Basic.Reference.Assemblies.Net110.References.All;
	private static IEnumerable<MetadataReference> AspNetCoreAssemblies => Basic.Reference.Assemblies.AspNet110.References.All;
#else
#error .net version not yet implemented
#endif

	public static IEnumerable<MetadataReference> GetMetadataReferences() =>
	[
		.. AspNetCoreAssemblies,

		MetadataReference.CreateFromFile(GetAssemblyLocation(typeof(HandlerAttribute))),
		MetadataReference.CreateFromFile(GetAssemblyLocation(typeof(MapMethodAttribute))),

		MetadataReference.CreateFromFile(GetAssemblyLocation(typeof(IServiceCollection))),
		MetadataReference.CreateFromFile(GetAssemblyLocation(typeof(ServiceCollection))),
		MetadataReference.CreateFromFile(GetAssemblyLocation(typeof(ILogger))),
		MetadataReference.CreateFromFile(GetAssemblyLocation(typeof(IOptions<>))),
		MetadataReference.CreateFromFile(GetAssemblyLocation(typeof(ChangeToken))),

		MetadataReference.CreateFromFile(GetAssemblyLocation(typeof(AuthorizeAttribute))),
		MetadataReference.CreateFromFile(GetAssemblyLocation(typeof(IAllowAnonymous))),
	];

	private static string GetAssemblyLocation(this Type type)
	{
		if (Assembly.GetAssembly(type) is not { Location: { } location })
			throw new InvalidOperationException("Missing assembly");

		return location;
	}

	public static TheoryData<string> Methods() =>
		[
			"Get",
			"Post",
			"Patch",
			"Put",
			"Delete",
		];
}
