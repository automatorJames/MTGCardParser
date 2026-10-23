namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Light from Within, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Light from Within</exampledoc>
[TokenizationOrder(-1)]
public class LightFromWithinRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["chroma — each creature you control gets +1/+1 for each white mana symbol in its mana cost"];
}
