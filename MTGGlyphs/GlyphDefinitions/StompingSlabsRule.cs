namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Stomping Slabs, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Stomping Slabs</exampledoc>
[TokenizationOrder(-1)]
public class StompingSlabsRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if a card named", Nib.This, "was revealed this way,", Nib.This, "deals 7 damage to any target"];
}
