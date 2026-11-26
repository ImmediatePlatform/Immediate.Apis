using System.Xml;
using System.Xml.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Immediate.Apis.Generators;

public sealed partial class ImmediateApisGenerator
{
	private static EndpointDocumentation GetEndpointDocumentation(
		INamedTypeSymbol handler,
		IMethodSymbol method,
		string parameterAttribute,
		Compilation compilation,
		CancellationToken token
	)
	{
		var handlerXml = ReadDocumentation(handler, token);
		var methodXml = ReadDocumentation(method, token);
		var request = method.Parameters[0];
		var requestXml = ReadDocumentation(request.Type, token);
		var parameters = new Dictionary<string, string>(StringComparer.Ordinal);
		var responses = new Dictionary<string, string>(StringComparer.Ordinal);
		string? requestBody = null;

		if (parameterAttribute is "global::Microsoft.AspNetCore.Http.AsParametersAttribute"
			&& request.Type is INamedTypeSymbol requestType)
		{
			for (INamedTypeSymbol? type = requestType; type is not null; type = type.BaseType)
			{
				var typeXml = SymbolEqualityComparer.Default.Equals(type, requestType)
					? requestXml
					: ReadDocumentation(type, token);
				foreach (var property in type.GetMembers().OfType<IPropertySymbol>())
				{
					token.ThrowIfCancellationRequested();
					if (property is { IsStatic: true } or { IsIndexer: true } or { GetMethod: not { DeclaredAccessibility: Accessibility.Public } }
						|| parameters.ContainsKey(property.Name))
					{
						continue;
					}

					var propertyXml = ReadDocumentation(property, token);
					var parameterXml = typeXml?.Elements("param")
						.FirstOrDefault(element => HasAttributeValue(element, "name", property.Name));
					if ((GetCommentText(propertyXml?.Element("summary"), compilation)
						?? GetCommentText(parameterXml, compilation)) is { } description)
					{
						parameters.Add(property.Name, description);
					}
				}
			}
		}
		else if (parameterAttribute is "global::Microsoft.AspNetCore.Mvc.FromBodyAttribute" or "global::Microsoft.AspNetCore.Mvc.FromFormAttribute")
		{
			var parameterXml = methodXml?.Elements("param")
				.FirstOrDefault(element => HasAttributeValue(element, "name", request.Name));
			requestBody = GetCommentText(parameterXml, compilation) ?? GetCommentText(requestXml?.Element("summary"), compilation);
		}

		foreach (var element in (handlerXml?.Elements("response") ?? []).Concat(methodXml?.Elements("response") ?? []))
		{
			if (element.Attribute("code")?.Value is { Length: > 0 } code && GetCommentText(element, compilation) is { } text)
				responses[code] = text;
		}

		return new(
			Summary: HasEndpointMetadata(method, compilation, "Microsoft.AspNetCore.Http.Metadata.IEndpointSummaryMetadata")
				? null
				: StringLiteral(GetCommentText(handlerXml?.Element("summary"), compilation) ?? GetCommentText(methodXml?.Element("summary"), compilation)),
			Description: HasEndpointMetadata(method, compilation, "Microsoft.AspNetCore.Http.Metadata.IEndpointDescriptionMetadata")
				? null
				: StringLiteral(GetCommentText(handlerXml?.Element("remarks"), compilation) ?? GetCommentText(methodXml?.Element("remarks"), compilation)),
			Parameters: GetDocumentationEntries(parameters),
			RequestBody: StringLiteral(requestBody),
			Returns: StringLiteral(GetCommentText(methodXml?.Element("returns"), compilation) ?? GetCommentText(handlerXml?.Element("returns"), compilation)),
			Responses: GetDocumentationEntries(responses)
		);
	}

	private static bool HasEndpointMetadata(IMethodSymbol method, Compilation compilation, string metadataName)
	{
		var metadataType = compilation.GetTypeByMetadataName(metadataName);
		return method.GetAttributes().Any(attribute =>
			attribute.AttributeClass?.AllInterfaces.Any(type => SymbolEqualityComparer.Default.Equals(type, metadataType)) == true);
	}

	private static string? StringLiteral(string? value) => value is null ? null : SymbolDisplay.FormatLiteral(value, quote: true);

	private static EquatableReadOnlyList<DocumentationEntry> GetDocumentationEntries(Dictionary<string, string> comments) =>
		comments.Select(kvp => new DocumentationEntry(SymbolDisplay.FormatLiteral(kvp.Key, quote: true), SymbolDisplay.FormatLiteral(kvp.Value, quote: true)))
			.ToEquatableReadOnlyList();

	private static bool HasAttributeValue(XElement element, string name, string value) =>
		string.Equals(element.Attribute(name)?.Value, value, StringComparison.Ordinal);

	private static XElement? ReadDocumentation(ISymbol symbol, CancellationToken token)
	{
		var xml = symbol.GetDocumentationCommentXml(expandIncludes: true, cancellationToken: token);
		if (string.IsNullOrWhiteSpace(xml))
			return null;

		try
		{
			return XElement.Parse(xml, LoadOptions.PreserveWhitespace);
		}
		catch (XmlException)
		{
			// The C# compiler reports malformed documentation when XML documentation diagnostics are enabled.
			return null;
		}
	}

	private static string? GetCommentText(XElement? element, Compilation compilation)
	{
		if (element is null)
			return null;

		var text = string.Concat(element.Nodes().Select(node => GetCommentText(node, compilation)));
		return string.Join(" ", text.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries)).NullIf("");
	}

	private static string? GetCommentText(XNode node, Compilation compilation) =>
		node switch
		{
			XText text => text.Value,
			XElement element when element.Name.LocalName is "see" or "seealso" =>
				GetCommentText(element, compilation)
				?? element.Attribute("langword")?.Value
				?? element.Attribute("href")?.Value
				?? GetReferenceText(element.Attribute("cref")?.Value, compilation),
			XElement element when element.Name.LocalName is "paramref" or "typeparamref" => element.Attribute("name")?.Value,
			XElement element when element.Name.LocalName is "c" => $"`{GetCommentText(element, compilation)}`",
			XElement element when element.Name.LocalName is "para" or "br" => $" {GetCommentText(element, compilation)} ",
			XElement element => GetCommentText(element, compilation),
			_ => null,
		};

	private static string? GetReferenceText(string? reference, Compilation compilation) =>
		reference is null
			? null
			: DocumentationCommentId.GetFirstSymbolForDeclarationId(reference, compilation)
				?.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat)
				?? (reference is [_, ':', ..] ? reference[2..] : reference);
}
