namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Gideon Blackblade, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Gideon Blackblade</exampledoc>
[TokenizationOrder(-1)]
public class GideonBlackbladeRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["during your turn,", Nib.This, "is a 4/4 human soldier creature with indestructible that's still a planeswalker"];
}
