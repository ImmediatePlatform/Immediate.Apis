using System.Collections.Immutable;

namespace Immediate.Apis.Generators;

public sealed partial class ImmediateApisGenerator
{
	private sealed record ResolvedRouting
	{
		public required EquatableReadOnlyList<Method> Endpoints { get; init; }
		public required EquatableReadOnlyList<RouteGroupDefinition> Groups { get; init; }
	}

	private static ResolvedRouting ResolveRouting(
		ImmutableArray<Method> endpoints,
		ImmutableArray<RouteGroupDefinition> groups,
		FileSystemRoutingSettings settings, CancellationToken token)
	{
		if (!settings.Enabled)
			return new()
			{
				Endpoints = endpoints.Where(e => e.Routes.Count > 0).ToEquatableReadOnlyList(),
				Groups = groups.ToEquatableReadOnlyList(),
			};

		var resolver = new FileSystemRouting(settings, [.. groups.Select(g =>
			new FileSystemGroup
			{
				ClassName = g.ClassFullName!,
				ParentClassName = g.DeclarationParentClassFullName,
				FilePath = g.FilePath,
				Route = g.Route,
			})]);
		var resolved = new List<Method>();
		foreach (var endpoint in endpoints.OrderBy(e => e.ClassFullName, StringComparer.Ordinal))
		{
			token.ThrowIfCancellationRequested();
			if (endpoint.Routes.Count > 0)
			{
				resolved.Add(endpoint);
				continue;
			}

			var route = endpoint.RouteGroupClassFullName is { } explicitGroup
				? new FileSystemRoute { GroupClassName = explicitGroup, Error = null }
				: resolver.Resolve(endpoint.FilePath, endpoint.PartialFiles);
			if (route.Error is null)
				resolved.Add(endpoint with { Routes = new([""]), RouteGroupClassFullName = route.GroupClassName });
		}

		var definitions = groups.ToDictionary(g => g.ClassFullName!, StringComparer.Ordinal);
		foreach (var directory in resolver.Groups)
		{
			token.ThrowIfCancellationRequested();
			if (directory.IsDeclared)
			{
				definitions[directory.ClassName] = definitions[directory.ClassName] with { RouteGroupClassFullName = directory.ParentClassName };
				continue;
			}

			definitions.Add(directory.ClassName, new()
			{
				FilePath = "",
				DeclarationParentClassFullName = null,
				Namespace = "Immediate.Apis.Generated",
				Class = new() { Type = "class", Name = directory.ClassName[(directory.ClassName.LastIndexOf('.') + 1)..] },
				ClassFullName = directory.ClassName,
				RouteGroupClassFullName = directory.ParentClassName,
				Route = directory.Route,
				UseCustomization = false,
				Tags = null,
			});
		}

		return new()
		{
			Endpoints = resolved.ToEquatableReadOnlyList(),
			Groups = definitions.Values.OrderBy(g => g.ClassFullName, StringComparer.Ordinal).ToEquatableReadOnlyList(),
		};
	}
}
