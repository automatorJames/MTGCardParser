namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Tatsunari, Toad Rider, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Tatsunari, Toad Rider</exampledoc>
/// <examplecapture>if you don't control a creature named keimi, create keimi, a legendary 3/3 black and green frog creature token with "whenever you cast an enchantment spell, each opponent loses 1 life and you gain 1 life</examplecapture>
[Dependent]
public class TatsunariToadRiderEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if you don't control a creature named keimi, create keimi, a legendary 3/3 black and green frog creature token with \"whenever you cast an enchantment spell, each opponent loses 1 life and you gain 1 life"];
}
