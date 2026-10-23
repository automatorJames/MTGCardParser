namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Eye of Yawgmoth, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Eye of Yawgmoth</exampledoc>
[TokenizationOrder(-1)]
public class EyeOfYawgmothRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["put one into your hand and exile the rest"];
}
