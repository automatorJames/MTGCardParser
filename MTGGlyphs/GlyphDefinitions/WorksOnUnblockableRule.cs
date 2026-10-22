namespace MTGGlyphs.GlyphDefinitions;

/// <summary>The reminder on "becomes blocked" spells: "this spell works on creatures that can't be blocked".</summary>
/// <exampledoc>Fog Patch</exampledoc>
/// <examplecapture>this spell works on creatures that can't be blocked</examplecapture>
public class WorksOnUnblockableRule : Glyph
{
    public override Nib[] Nibs => ["this spell works on creatures that can't be blocked"];
}
