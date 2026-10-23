namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Sen Triplets, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Sen Triplets</exampledoc>
[TokenizationOrder(-1)]
public class SenTripletsRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["this turn, that player can't cast spells or activate abilities and plays with their hand revealed"];
}
