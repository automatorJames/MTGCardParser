using Glyphotype.Distiller.Workbench;
using Glyphotype.Distiller.Workspaces;

namespace DocumentAnalysisInterface.Components.CorpusCaptures;

/// <summary>
/// Lets a property table show the glyph or vocabulary each header part or row names, as the Grammar Tools page shows its
/// definitions: a preview of the definition while one is hovered (see <see cref="DefinitionTooltip"/>), and the
/// definition viewer when it's clicked. Cascaded, fixed, from the Corpus Captures page to every table; the viewer itself
/// is <see cref="GlyphPreviewViewer"/>'s, so opening it re-renders only that rather than every document on the page.
/// </summary>
public sealed class GlyphPreviews(WorkspaceManager workspaces)
{
    /// <summary>Raised with a definition's kind and name when it's to be shown in the viewer.</summary>
    public event Action<DefinitionKind, string> ViewRequested;

    /// <summary>
    /// Which kind of definition the active workspace has for <paramref name="type"/> - a glyph, or a vocabulary (an enum
    /// it defines) - or null for one it doesn't define: a primitive, or a built-in pronoun, which have no definition to show.
    /// </summary>
    public DefinitionKind? KindOf(Type type)
    {
        if (type is null)
            return null;

        var working = workspaces.Active.WorkingDefinition;
        var committed = workspaces.Active.CommittedDefinition;

        if (working.Glyphs.Any(x => x.Name == type.Name) || committed.Glyphs.Any(x => x.Name == type.Name))
            return DefinitionKind.Glyph;

        if (type.IsEnum && (working.Vocabularies.Any(x => x.Name == type.Name) || committed.Vocabularies.Any(x => x.Name == type.Name)))
            return DefinitionKind.Vocabulary;

        return null;
    }

    /// <summary>Whether <paramref name="type"/> has a definition to show (see <see cref="KindOf"/>).</summary>
    public bool Has(Type type) => KindOf(type) is not null;

    public void View(Type type)
    {
        if (KindOf(type) is { } kind)
            ViewRequested?.Invoke(kind, type.Name);
    }
}
