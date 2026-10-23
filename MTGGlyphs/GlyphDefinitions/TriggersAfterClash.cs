namespace MTGGlyphs.GlyphDefinitions;

/// <summary>The rules note on a clash trigger: "this ability triggers after the clash ends".</summary>
/// <exampledoc>Sylvan Echoes</exampledoc>
/// <examplecapture>this ability triggers after the clash ends</examplecapture>
[Dependent]
public class TriggersAfterClash : Glyph
{
    public override Nib[] Nibs => ["this ability triggers after the clash ends"];
}
