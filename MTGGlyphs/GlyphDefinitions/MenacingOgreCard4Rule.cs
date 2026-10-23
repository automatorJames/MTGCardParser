namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Menacing Ogre, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Menacing Ogre</exampledoc>
[TokenizationOrder(-1)]
public class MenacingOgreCard4Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["then those numbers are revealed"];
}
