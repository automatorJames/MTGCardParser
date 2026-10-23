namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Ward of Bones, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Ward of Bones</exampledoc>
[TokenizationOrder(-1)]
public class WardOfBones2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["each opponent who controls more lands than you can't play lands"];
}
