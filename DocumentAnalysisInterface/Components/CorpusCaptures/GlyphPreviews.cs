using Glyphotype.Distiller.Workspaces;

namespace DocumentAnalysisInterface.Components.CorpusCaptures;

/// <summary>
/// Lets a property table's headers show the glyph each part names, as the Grammar Tools page shows its glyphs: a preview
/// of the definition while one is hovered (see <see cref="DefinitionTooltip"/>), and the definition viewer when it's
/// clicked. Cascaded, fixed, from the Corpus Captures page to every table; the viewer itself is
/// <see cref="GlyphPreviewViewer"/>'s, so opening it re-renders only that rather than every document on the page.
/// </summary>
public sealed class GlyphPreviews(WorkspaceManager workspaces)
{
    /// <summary>Raised with a glyph's name when its definition is to be shown in the viewer.</summary>
    public event Action<string> ViewRequested;

    /// <summary>Whether <paramref name="type"/> is a glyph the active workspace defines - not a primitive or a built-in pronoun, which have no definition to show.</summary>
    public bool Has(Type type) =>
        type is not null
        && (workspaces.Active.WorkingDefinition.Glyphs.Any(x => x.Name == type.Name)
            || workspaces.Active.CommittedDefinition.Glyphs.Any(x => x.Name == type.Name));

    public void View(Type type) => ViewRequested?.Invoke(type.Name);
}
