namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Kroxa, Titan of Death's Hunger, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Kroxa, Titan of Death's Hunger</exampledoc>
[Dependent]
public class KroxaTitanOfDeathsHungerEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["each opponent discards a card, then each opponent who didn't discard a nonland card this way loses 3 life"];
}
