namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Tallowisp, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Tallowisp</exampledoc>
/// <examplecapture>you may search your library for an aura card with enchant creature, reveal it, put it into your hand, then shuffle</examplecapture>
[Dependent]
public class TallowispEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may search your library for an aura card with enchant creature, reveal it, put it into your hand, then shuffle"];
}
