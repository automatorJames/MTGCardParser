namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Overlord of the Balemurk, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Overlord of the Balemurk</exampledoc>
/// <examplecapture>mill four cards, then you may return a non-avatar creature card or a planeswalker card from your graveyard to your hand</examplecapture>
[Dependent]
public class OverlordOfTheBalemurkEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["mill four cards, then you may return a non-avatar creature card or a planeswalker card from your graveyard to your hand"];
}
