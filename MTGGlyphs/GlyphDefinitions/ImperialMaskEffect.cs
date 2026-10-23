namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Imperial Mask, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Imperial Mask</exampledoc>
[Dependent]
public class ImperialMaskEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if it's not a token, each of your teammates creates a token that's a copy of this enchantment"];
}
