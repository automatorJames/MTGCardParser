using DocumentAnalysisInterface.Components.Regex;
using Glyphotype.Definitions;
using Glyphotype.Distiller.Workbench;
using Glyphotype.GlyphAnalysisDTOs.TypeExpressions;
using Glyphotype.RegexGeneration.Presentation;

namespace DocumentAnalysisInterface.Dialogs;

/// <summary>The ways <see cref="DefinitionViewerDialog"/> shows a definition, one per tab.</summary>
public enum DefinitionView { CSharp, Json, FormattedRegex, MinifiedRegex, Diff }

/// <summary>
/// A definition as each <see cref="DefinitionView"/> shows it, as plain text: what the viewer's copy buttons copy,
/// and what a definition's tooltip previews.
/// </summary>
public static class DefinitionViews
{
    const int PreviewLines = 24;
    const int PreviewLineLength = 140;
    const int PreviewLength = 1600;

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
            GlyphDefinition glyph => GlyphSourceWriter.WriteGlyph(glyph),
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

    /// <summary>
    /// <paramref name="view"/>'s text for a definition (falling back to C# where it doesn't have that view), cut down
    /// to a tooltip's size. <paramref name="glyphType"/> is the glyph's built type, or null if there's none to hand.
    /// </summary>
    public static string Preview(DefinitionView view, DefinitionKind kind, ChangeType? change, object committed, object working, Type glyphType, bool includeBlankLines)
    {
        var shown = working ?? committed;

        if (!For(kind, change).Contains(view))
            view = DefinitionView.CSharp;

        string text;

        try
        {
            text = view switch
            {
                DefinitionView.Json => Json(shown),
                DefinitionView.FormattedRegex => glyphType is null ? "(the regex shows once the grammar is built)" : FormattedRegexText(glyphType, includeBlankLines),
                DefinitionView.MinifiedRegex => glyphType is null ? "(the regex shows once the grammar is built)" : MinifiedRegexText(glyphType),
                DefinitionView.Diff => DiffText(LineDiff.Compare(CSharp(committed), CSharp(working))),
                _ => CSharp(shown),
            };
        }
        catch (Exception exception)
        {
            text = $"(couldn't show the {Label(view)}: {exception.Message})";
        }

        return Truncate(text);
    }

    /// <summary>At most <see cref="PreviewLines"/> lines of at most <see cref="PreviewLineLength"/> characters, <see cref="PreviewLength"/> in all - marking each cut with an ellipsis.</summary>
    static string Truncate(string text)
    {
        var lines = text.Replace("\r\n", "\n").Split('\n');
        var kept = new List<string>();
        var length = 0;
        var cut = false;

        foreach (var line in lines)
        {
            if (kept.Count == PreviewLines || length >= PreviewLength)
            {
                cut = true;
                break;
            }

            var shortened = line.Length > PreviewLineLength ? line[..(PreviewLineLength - 1)] + "…" : line;

            if (length + shortened.Length > PreviewLength)
            {
                shortened = shortened[..Math.Max(0, PreviewLength - length)] + "…";
                cut = true;
            }

            kept.Add(shortened);
            length += shortened.Length + 1;

            if (cut)
                break;
        }

        return string.Join("\n", kept) + (cut && !kept[^1].EndsWith('…') ? "\n…" : "");
    }
}
