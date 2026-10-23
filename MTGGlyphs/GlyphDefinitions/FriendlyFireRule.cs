namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Friendly Fire, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Friendly Fire</exampledoc>
[TokenizationOrder(-1)]
public class FriendlyFireRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => [Nib.This, "deals damage to that creature and that player equal to the revealed card's mana value"];
}
