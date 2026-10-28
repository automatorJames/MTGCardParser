namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Ice Cauldron, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Ice Cauldron</exampledoc>
[TokenizationOrder(-1)]
public class IceCauldronRule2 : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["spend this mana only to cast the last card exiled with this artifact"];
}
