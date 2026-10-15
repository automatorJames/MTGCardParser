namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Clash's reminder text.</summary>
/// <exampledoc>Ember Weaver</exampledoc>
/// <examplecapture>each clashing player reveals the top card of their library, then puts that card on the top or bottom</examplecapture>
public class ClashRule : Glyph
{
    public override Nib[] Nibs => ["each clashing player reveals the top card of their library, then puts that card on the top or bottom"];
}
