namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Human Torch, Johnny Storm, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Human Torch, Johnny Storm</exampledoc>
[Dependent]
public class HumanTorchJohnnyStormEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if you control another hero, human torch deals 1 damage to target opponent"];
}
