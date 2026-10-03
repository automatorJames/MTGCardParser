namespace Glyphotype.Definitions;

/// <summary>
/// Writes definitions as C# source in the style of hand-written glyph files: file-scoped namespace, no usings
/// (glyph projects supply Glyphotype's namespaces as global usings), attributes on their own lines, and nibs
/// through the <see cref="Glyph"/> helpers (<c>Prop</c>, <c>Alt</c>, <c>Opt</c>, <c>Plural</c>, <c>Pattern</c>)
/// exactly as an author would write them.
/// </summary>
public static class GlyphSourceWriter
{
    const string _indent = "    ";

    /// <summary>One source file declaring every marker, vocabulary and glyph in <paramref name="grammar"/>.</summary>
    public static string Write(GrammarDefinition grammar, string @namespace)
    {
        var declarations = grammar.Markers.Select(WriteMarker)
            .Concat(grammar.Vocabularies.Select(WriteVocabulary))
            .Concat(grammar.Glyphs.Select(WriteGlyph));

        return $"namespace {@namespace};{Environment.NewLine}{Environment.NewLine}" + string.Join(Environment.NewLine, declarations);
    }

    public static string WriteMarker(string name) =>
        Lines($"public interface {name}", "{", "}");

    public static string WriteVocabulary(VocabularyDefinition vocabulary)
    {
        List<string> lines = [];

        if (vocabulary.IsOptionalPlural)
            lines.Add("[OptionalPlural]");

        lines.Add($"public enum {vocabulary.Name}");
        lines.Add("{");

        // Hand-written vocabularies separate members with a blank line once any member carries attributes.
        var spaced = vocabulary.Members.Any(x => x.Patterns.Count > 0 || x.Color is not null);

        for (int i = 0; i < vocabulary.Members.Count; i++)
        {
            var member = vocabulary.Members[i];

            if (spaced && i > 0)
                lines.Add("");

            if (member.Patterns.Count > 0)
                lines.Add(_indent + RegexPatternAttribute(member.Patterns));

            if (member.Color is not null)
                lines.Add(_indent + $"[Color({Literal(member.Color)})]");

            var value = member.Value is long explicitValue ? $" = {explicitValue}" : "";
            var separator = i < vocabulary.Members.Count - 1 ? "," : "";
            lines.Add(_indent + member.Name + value + separator);
        }

        lines.Add("}");
        return Lines([.. lines]);
    }

    public static string WriteGlyph(GlyphDefinition glyph)
    {
        List<string> lines = [.. GetClassAttributes(glyph)];

        var baseList = string.Join(", ", glyph.Markers.Prepend(glyph.Kind switch
        {
            GlyphKind.Glyph => nameof(Glyph),
            GlyphKind.GlyphOneOf => nameof(GlyphOneOf),
            GlyphKind.BackReference => nameof(BackReference),
            _ => glyph.AliasOf.ToString(),
        }));

        var header = $"public class {glyph.Name} : {baseList}";

        List<string> overrides = [];

        if (glyph.Joiner is Joiner joiner)
            overrides.Add(_indent + $"public override Joiner Joiner => Joiner.{joiner};");

        if (glyph.Nibs.Count > 0)
            overrides.Add(_indent + $"public override Nib[] Nibs => [{string.Join(", ", glyph.Nibs.Select(WriteNib))}];");

        var properties = glyph.Properties.SelectMany(WriteProperty).ToList();

        if (overrides.Count == 0 && properties.Count == 0)
        {
            lines.Add(header + ";");
            return Lines([.. lines]);
        }

        lines.Add(header);
        lines.Add("{");
        lines.AddRange(overrides);

        if (overrides.Count > 0 && properties.Count > 0)
            lines.Add("");

        lines.AddRange(properties);
        lines.Add("}");

        return Lines([.. lines]);
    }

    static IEnumerable<string> GetClassAttributes(GlyphDefinition glyph)
    {
        if (glyph.IsDependent)
            yield return "[Dependent]";

        if (glyph.SpanRule == SpanRule.PartialClause)
            yield return "[AllowPartialClauseMatch]";

        if (glyph.TokenizationOrder is int order)
            yield return $"[TokenizationOrder({order})]";

        if (glyph.Patterns.Count > 0)
            yield return RegexPatternAttribute(glyph.Patterns);

        if (glyph.JoinedBy is Joiner joinedBy)
            yield return $"[JoinedBy(Joiner.{joinedBy})]";

        if (glyph.Introduces is { } introduces)
            yield return AgreementAttribute("Introduces", introduces);

        if (glyph.Agreement is { } agreement)
            yield return AgreementAttribute("Agreement", agreement);
    }

    /// <summary>
    /// <c>[Introduces]</c> or <c>[Agreement]</c> with only the arguments it needs: <c>[Introduces]</c>,
    /// <c>[Introduces(GrammaticalNumber.Plural)]</c>, <c>[Agreement(GrammaticalNumber.Singular, "creature")]</c>.
    /// </summary>
    static string AgreementAttribute(string name, AgreementDefinition features)
    {
        var number = $"{nameof(GrammaticalNumber)}.{features.Number}";

        return features switch
        {
            { Kind: not null } => $"[{name}({number}, {Literal(features.Kind)})]",
            { Number: not GrammaticalNumber.Unspecified } => $"[{name}({number})]",
            _ => $"[{name}]",
        };
    }

    static IEnumerable<string> WriteProperty(PropertyDefinition property)
    {
        if (property.IsOptional)
            yield return _indent + "[Optional]";

        if (property.AllowsUnmatched)
            yield return _indent + "[AllowUnmatched]";

        if (property.Patterns.Count > 0)
            yield return _indent + RegexPatternAttribute(property.Patterns);

        if (property.JoinedBy is Joiner joinedBy)
            yield return _indent + $"[JoinedBy(Joiner.{joinedBy})]";

        if (property.TypeFilter is not null)
            yield return _indent + $"[TypeFilter(typeof({property.TypeFilter}))]";

        if (property.Introduces is { } introduces)
            yield return _indent + AgreementAttribute("Introduces", introduces);

        if (property.RefersTo is not null)
            yield return _indent + $"[RefersTo(nameof({property.RefersTo}))]";

        yield return _indent + $"public {property.Type} {property.Name} {{ get; set; }}";
    }

    static string WriteNib(NibDefinition nib) =>
        nib switch
        {
            NibDefinition.Literal literal => Literal(literal.Text),
            NibDefinition.Pattern pattern => $"Pattern({Literal(pattern.Regex)})",
            NibDefinition.Alternatives alternatives => $"Alt({string.Join(", ", alternatives.Texts.Select(Literal))})",
            NibDefinition.Optional optional => $"Opt({WriteNib(optional.Inner)})",
            NibDefinition.Plural => "Plural()",
            NibDefinition.Property { Proptions: Proptions.None } property => $"Prop({property.Name})",
            NibDefinition.Property property => $"Prop({property.Name}, {WriteProptions(property.Proptions)})",
            _ => throw new NotSupportedException($"Nib definition {nib.GetType().Name} can't be written"),
        };

    static string WriteProptions(Proptions proptions) =>
        string.Join(" | ", Enum.GetValues<Proptions>().Where(x => x != Proptions.None && proptions.HasFlag(x)).Select(x => $"{nameof(Proptions)}.{x}"));

    static string RegexPatternAttribute(IEnumerable<string> patterns) =>
        $"[RegexPattern({string.Join(", ", patterns.Select(Literal))})]";

    /// <summary>
    /// <paramref name="text"/> as a C# string literal: verbatim when it holds a backslash (so a regex reads as
    /// written, e.g. <c>@"\d+"</c>) and nothing a verbatim string can't hold, else a regular escaped one.
    /// </summary>
    static string Literal(string text)
    {
        if (text.Contains('\\') && !text.Any(char.IsControl))
            return $"@\"{text.Replace("\"", "\"\"")}\"";

        var escaped = new StringBuilder("\"");

        foreach (var c in text)
            escaped.Append(c switch
            {
                '"' => "\\\"",
                '\\' => "\\\\",
                '\n' => "\\n",
                '\r' => "\\r",
                '\t' => "\\t",
                _ when char.IsControl(c) => $"\\u{(int)c:x4}",
                _ => c.ToString(),
            });

        return escaped.Append('"').ToString();
    }

    static string Lines(params string[] lines) =>
        string.Concat(lines.Select(x => x + Environment.NewLine));
}
