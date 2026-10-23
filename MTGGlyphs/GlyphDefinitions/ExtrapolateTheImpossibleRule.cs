namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Extrapolate the Impossible, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Extrapolate the Impossible</exampledoc>
[TokenizationOrder(-1)]
public class ExtrapolateTheImpossibleRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may reveal exactly two cards you own with different names from outside the game"];
}
