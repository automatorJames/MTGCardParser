namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Human Torch, Johnny Storm, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Human Torch, Johnny Storm</exampledoc>
[TokenizationOrder(-1)]
public class HumanTorchJohnnyStormRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["whenever you draw a card, if you control another hero, human torch deals 1 damage to target opponent"];
}
