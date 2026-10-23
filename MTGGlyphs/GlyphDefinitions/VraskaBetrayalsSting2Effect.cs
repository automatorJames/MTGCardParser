namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Vraska, Betrayal's Sting, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Vraska, Betrayal's Sting</exampledoc>
[Dependent]
public class VraskaBetrayalsSting2Effect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if target player has fewer than nine poison counters, they get a number of poison counters equal to the difference"];
}
