namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Thousand-Faced Shadow, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Thousand-Faced Shadow</exampledoc>
[TokenizationOrder(-1)]
public class ThousandFacedShadowCardRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["when this creature enters from your hand, if it's attacking, create a token that's a copy of another target attacking creature"];
}
