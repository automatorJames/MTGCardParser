namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Warp's reminder text, first sentence.</summary>
/// <exampledoc>Starfield Shepherd</exampledoc>
/// <examplecapture>you may cast this card from your hand for its warp cost</examplecapture>
public class WarpRule : Glyph
{
    public override Nib[] Nibs => ["you may cast this card from your hand for its warp cost"];
}
