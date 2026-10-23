namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Ice Cauldron, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Ice Cauldron</exampledoc>
[TokenizationOrder(-1)]
public class IceCauldronRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["put a charge counter on this artifact and note the type and amount of mana spent to pay this activation cost"];
}
