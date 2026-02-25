using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;

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
#else
#error .net version not yet implemented
#endif

	public static IEnumerable<MetadataReference> GetMetadataReferences() =>
	[
		.. AspNetCoreAssemblies,
		MetadataReference.CreateFromFile("./Immediate.Handlers.Shared.dll"),
		MetadataReference.CreateFromFile("./Immediate.Apis.Shared.dll"),
		MetadataReference.CreateFromFile("./Microsoft.Extensions.DependencyInjection.dll"),
		MetadataReference.CreateFromFile("./Microsoft.Extensions.DependencyInjection.Abstractions.dll"),
		MetadataReference.CreateFromFile("./Microsoft.AspNetCore.Authorization.dll"),
		MetadataReference.CreateFromFile("./Microsoft.AspNetCore.Metadata.dll"),
		MetadataReference.CreateFromFile("./Microsoft.Extensions.Logging.Abstractions.dll"),
		MetadataReference.CreateFromFile("./Microsoft.Extensions.Options.dll"),
		MetadataReference.CreateFromFile("./Microsoft.Extensions.Primitives.dll"),
	];

	public static TheoryData<string> Methods() =>
		[
			"Get",
			"Post",
			"Patch",
			"Put",
			"Delete",
		];
}
