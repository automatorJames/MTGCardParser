namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Improvisation Capstone, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Improvisation Capstone</exampledoc>
[TokenizationOrder(-1)]
public class ImprovisationCapstoneRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["exile cards from the top of your library until you exile cards with total mana value 4 or greater"];
}
