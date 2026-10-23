namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Invasive Surgery, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Invasive Surgery</exampledoc>
/// <examplecapture>delirium — if there are four or more card types among cards in your graveyard, search the graveyard, hand, and library of that spell's controller for any number of cards with the same name as that spell, exile those cards, then that player shuffles</examplecapture>
[TokenizationOrder(-1)]
public class InvasiveSurgeryRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["delirium — if there are four or more card types among cards in your graveyard, search the graveyard, hand, and library of that spell's controller for any number of cards with the same name as that spell, exile those cards, then that player shuffles"];
}
