namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Beastmaster's Magemark, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Beastmaster's Magemark</exampledoc>
[TokenizationOrder(-1)]
public class BeastmastersMagemarkRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["whenever a creature you control that's enchanted becomes blocked, it gets +1/+1 until end of turn for each creature blocking it"];
}
