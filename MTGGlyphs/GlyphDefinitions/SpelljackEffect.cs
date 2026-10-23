namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Spelljack, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Spelljack</exampledoc>
[Dependent]
public class SpelljackEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if it has x in its mana cost, x is 0"];
}
