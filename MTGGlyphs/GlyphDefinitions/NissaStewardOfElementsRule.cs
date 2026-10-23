namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Nissa, Steward of Elements, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Nissa, Steward of Elements</exampledoc>
/// <examplecapture>if it's a land card or a creature card with mana value less than or equal to the number of loyalty counters on nissa, you may put that card onto the battlefield</examplecapture>
[TokenizationOrder(-1)]
public class NissaStewardOfElementsRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if it's a land card or a creature card with mana value less than or equal to the number of loyalty counters on nissa, you may put that card onto the battlefield"];
}
