namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Demonlord Belzenlok, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Demonlord Belzenlok</exampledoc>
[TokenizationOrder(-1)]
public class DemonlordBelzenlokRule2 : Glyph, ICardSpecific
{
    public override Nib[] Nibs => [Nib.This, "deals 1 damage to you for each card put into your hand this way"];
}
