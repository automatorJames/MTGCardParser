namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Phasing's reminder text.</summary>
/// <exampledoc>Mist Dragon</exampledoc>
/// <examplecapture>this phases in or out before you untap during each of your untap steps</examplecapture>
public class PhasingRule : Glyph
{
    public override Nib[] Nibs => ["this phases in or out before you untap during each of your untap steps"];
}
