namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Charmed Pendant, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Charmed Pendant</exampledoc>
[Dependent]
public class CharmedPendantEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["for each colored mana symbol in the milled card's mana cost, add one mana of that color"];
}
