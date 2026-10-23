namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Rhystic Lightning, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Rhystic Lightning</exampledoc>
[TokenizationOrder(-1)]
public class RhysticLightningCard2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if they do,", Nib.This, "deals 2 damage to the permanent or player"];
}
