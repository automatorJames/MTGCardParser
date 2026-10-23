namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of The Eagles Are Coming!, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>The Eagles Are Coming!</exampledoc>
[TokenizationOrder(-1)]
public class TheEaglesAreComingCard2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["return each chosen creature to your hand"];
}
