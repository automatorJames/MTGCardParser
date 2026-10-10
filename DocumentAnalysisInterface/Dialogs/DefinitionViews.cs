using DocumentAnalysisInterface.Components.Regex;
using Glyphotype.Definitions;
using Glyphotype.Distiller.Workbench;
using Glyphotype.GlyphAnalysisDTOs.TypeExpressions;
using Glyphotype.RegexGeneration.Presentation;

namespace DocumentAnalysisInterface.Dialogs;

/// <summary>The ways <see cref="DefinitionViewerDialog"/> shows a definition, one per tab.</summary>
public enum DefinitionView { CSharp, Json, FormattedRegex, MinifiedRegex, Diff }

/// <summary>
/// A definition as each <see cref="DefinitionView"/> shows it: as plain text, for the viewer's copy buttons, and
/// highlighted, for the viewer and a definition's tooltip.
/// </summary>
public static class DefinitionViews
{
    public static string Label(DefinitionView view) =>
        view switch
        {
            DefinitionView.CSharp => "C#",
            DefinitionView.Json => "JSON",
            DefinitionView.FormattedRegex => "Regex",
            DefinitionView.MinifiedRegex => "Minified regex",
            _ => "Diff",
        };

    /// <summary>The views a definition has: the regex ones only for a glyph, the diff only for one changed in place.</summary>
    public static IReadOnlyList<DefinitionView> For(DefinitionKind kind, ChangeType? change)
    {
        var views = new List<DefinitionView> { DefinitionView.CSharp, DefinitionView.Json };

        if (kind == DefinitionKind.Glyph)
            views.AddRange([DefinitionView.FormattedRegex, DefinitionView.MinifiedRegex]);

        if (change == ChangeType.Modified)
            views.Add(DefinitionView.Diff);

        return views;
    }

    public static string CSharp(object definition) =>
        definition switch
        {
            GlyphDefinition glyph => GlyphSourceWriter.WriteGlyph(glyph, spaceProperties: true),
            VocabularyDefinition vocabulary => GlyphSourceWriter.WriteVocabulary(vocabulary),
            _ => "",
        };

    public static string Json(object definition) =>
        definition is null ? "" : DefinitionJson.Serialize(definition);

    /// <summary>A glyph type's regex formatted as the Glyph Regex page's "full" format shows it: every vocabulary member, no corpus counts.</summary>
    public static SmartRegex FormattedRegex(Type glyphType, bool includeBlankLines)
    {
        var graph = GlyphTypeCache.GetRegexGraph(glyphType);
        return graph.BuiltRegex.ToSmartRegex(new GlyphOccurrenceSummary(glyphType), graph, includeBlankLines, RegexDisplayMode.Full);
    }

    public static string FormattedRegexText(Type glyphType, bool includeBlankLines) =>
        FormattedRegexCopy.GetCopyText(FormattedRegex(glyphType, includeBlankLines).Lines);

    public static string MinifiedRegexText(Type glyphType) =>
        GlyphTypeCache.GetRegexGraph(glyphType).BuiltRegex.MinifiedRegex;

    public static string DiffText(IEnumerable<LineDiff.Line> diff) =>
        string.Join("\n", diff.Select(x => x.Kind switch
        {
            LineDiff.LineKind.Added => "+ ",
            LineDiff.LineKind.Removed => "- ",
            _ => "  ",
        } + x.Text));

    /// <summary>A definition's C#, highlighted, its types colored by what <paramref name="resolveType"/> says they are.</summary>
    public static IReadOnlyList<CodeToken> CSharpTokens(object definition, Func<string, CodeTypeKind?> resolveType) =>
        SyntaxHighlighter.CSharp(CSharp(definition), resolveType);

    /// <summary>A definition's JSON, pretty-printed and highlighted.</summary>
    public static IReadOnlyList<CodeToken> JsonTokens(object definition) =>
        SyntaxHighlighter.JsonText(Json(definition));

    /// <summary>
    /// What each type a definition can name is - the grammar's glyphs classes, its vocabularies enums, its markers
    /// interfaces (from either grammar, so a removed one still reads right) - along with the types Glyphotype's own
    /// declarations use.
    /// </summary>
    public static Func<string, CodeTypeKind?> TypeResolver(params GrammarDefinition[] grammars)
    {
        var kinds = new Dictionary<string, CodeTypeKind>
        {
            ["Glyph"] = CodeTypeKind.Class,
            ["GlyphOneOf"] = CodeTypeKind.Class,
            ["Nib"] = CodeTypeKind.Class,
            ["Joiner"] = CodeTypeKind.Enum,
        };

        foreach (var grammar in grammars.Where(x => x is not null))
        {
            foreach (var glyph in grammar.Glyphs)
                kinds.TryAdd(glyph.Name, CodeTypeKind.Class);

            foreach (var vocabulary in grammar.Vocabularies)
                kinds.TryAdd(vocabulary.Name, CodeTypeKind.Enum);

            foreach (var marker in grammar.Markers)
                kinds.TryAdd(marker, CodeTypeKind.Interface);
        }

        return name => kinds.TryGetValue(name, out var kind) ? kind : null;
    }
}
