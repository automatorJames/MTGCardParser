namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of The Rollercrusher Ride, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>The Rollercrusher Ride</exampledoc>
/// <examplecapture>delirium — if a source you control would deal noncombat damage to a permanent or player while there are four or more card types among cards in your graveyard, it deals double that damage instead</examplecapture>
[TokenizationOrder(-1)]
public class TheRollercrusherRideRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["delirium — if a source you control would deal noncombat damage to a permanent or player while there are four or more card types among cards in your graveyard, it deals double that damage instead"];
}
