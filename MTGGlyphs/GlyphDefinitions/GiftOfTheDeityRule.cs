namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Gift of the Deity, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Gift of the Deity</exampledoc>
[TokenizationOrder(-1)]
public class GiftOfTheDeityRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["as long as enchanted creature is green, it gets +1/+1 and all creatures able to block it do so"];
}
