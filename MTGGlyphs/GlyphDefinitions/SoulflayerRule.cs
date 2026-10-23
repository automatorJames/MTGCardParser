namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Soulflayer, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Soulflayer</exampledoc>
[TokenizationOrder(-1)]
public class SoulflayerRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if a creature card with flying was exiled with this creature's delve ability, this creature has flying"];
}
