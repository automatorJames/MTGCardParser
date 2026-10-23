namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Rumbling Ruin, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Rumbling Ruin</exampledoc>
[TokenizationOrder(-1)]
public class RumblingRuinRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["creatures your opponents control with power less than or equal to that number can't block this turn"];
}
