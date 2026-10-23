namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Rith, Liberated Primeval, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Rith, Liberated Primeval</exampledoc>
/// <examplecapture>if a creature or planeswalker an opponent controlled was dealt excess damage this turn, create a 4/4 red dragon creature token with flying</examplecapture>
[Dependent]
public class RithLiberatedPrimevalEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if a creature or planeswalker an opponent controlled was dealt excess damage this turn, create a 4/4 red dragon creature token with flying"];
}
