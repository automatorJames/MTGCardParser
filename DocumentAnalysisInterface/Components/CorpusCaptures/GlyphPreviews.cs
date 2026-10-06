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
    /// Which kind of definition <paramref name="type"/> has - a glyph, or a vocabulary (an enum) - in the active workspace
    /// or among Glyphotype's built-ins (see <see cref="GrammarDefinition.BuiltIns"/>, e.g. the standard pronouns); null
    /// for one with none to show, such as a primitive.
    /// </summary>
    public DefinitionKind? KindOf(Type type)
    {
        if (type is null)
            return null;

        GrammarDefinition[] definitions = [workspaces.Active.WorkingDefinition, workspaces.Active.CommittedDefinition, GrammarDefinition.BuiltIns];

        if (definitions.Any(d => d.Glyphs.Any(x => x.Name == type.Name)))
            return DefinitionKind.Glyph;

        if (type.IsEnum && definitions.Any(d => d.Vocabularies.Any(x => x.Name == type.Name)))
            return DefinitionKind.Vocabulary;

        return null;
    }

    /// <summary>The built-in glyph type named <paramref name="name"/>, or null for none.</summary>
    public static Type BuiltInType(string name) =>
        GrammarDefinition.BuiltInNames.Contains(name) ? typeof(Glyphotype.GlyphPrimitives.Glyph).Assembly.GetExportedTypes().FirstOrDefault(x => x.Name == name) : null;

    /// <summary>Whether <paramref name="type"/> has a definition to show (see <see cref="KindOf"/>).</summary>
    public bool Has(Type type) => KindOf(type) is not null;

    public void View(Type type)
    {
        if (KindOf(type) is { } kind)
            ViewRequested?.Invoke(kind, type.Name);
    }
}
