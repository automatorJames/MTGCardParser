namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Solstice Revelations, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Solstice Revelations</exampledoc>
[TokenizationOrder(-1)]
public class SolsticeRevelationsRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may cast that card without paying its mana cost if the spell's mana value is less than the number of mountains you control"];
}
