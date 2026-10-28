namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Deification, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Deification</exampledoc>
/// <examplecapture>as long as you control a creature, if damage dealt to a planeswalker you control of the chosen type would result in all loyalty counters on it being removed, instead all but one of those counters are removed</examplecapture>
[TokenizationOrder(-1)]
public class DeificationRule2 : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["as long as you control a creature, if damage dealt to a planeswalker you control of the chosen type would result in all loyalty counters on it being removed, instead all but one of those counters are removed"];
}
