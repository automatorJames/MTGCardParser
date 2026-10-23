namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Tragic Arrogance, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Tragic Arrogance</exampledoc>
[TokenizationOrder(-1)]
public class TragicArroganceRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["for each player, you choose from among the permanents that player controls an artifact, a creature, an enchantment, and a planeswalker"];
}
