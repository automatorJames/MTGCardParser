namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Clash's reminder text, first sentence; current Oracle wording adds "their choice of".</summary>
/// <exampledoc>Adder-Staff Boggart</exampledoc>
/// <examplecapture>each clashing player reveals the top card of their library, then puts that card on their choice of the top or bottom</examplecapture>
public class ClashRule : Glyph
{
    public override Nib[] Nibs => ["each clashing player reveals the top card of their library, then puts that card on", Opt("their choice of"), "the top or bottom"];
}
