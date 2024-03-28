using Microsoft.CodeAnalysis;

namespace Immediate.Apis.Tests.Helpers;

public static class ReferenceAssemblyHelpers
{
	public static IEnumerable<MetadataReference> GetAdditionalReferences(this DriverReferenceAssemblies assemblies)
	{
		ArgumentNullException.ThrowIfNull(assemblies);

		List<MetadataReference> references =
		[
			MetadataReference.CreateFromFile("./Immediate.Handlers.Shared.dll"),
			MetadataReference.CreateFromFile("./Immediate.Apis.Shared.dll")
		];

		if (assemblies is DriverReferenceAssemblies.Normal)
			return references;

		// to be done with other renderers
		throw new NotImplementedException();
	}
}

public enum DriverReferenceAssemblies
{
	Normal
}
