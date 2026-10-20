namespace MTGGlyphs.GlyphDefinitions;

/// <summary>The reminder text of the fight keyword action.</summary>
/// <exampledoc>Prey Upon</exampledoc>
/// <examplecapture>each deals damage equal to its power to the other</examplecapture>
public class FightRule : Glyph
{
    public override Nib[] Nibs => ["each deals damage equal to its power to the other"];
}
