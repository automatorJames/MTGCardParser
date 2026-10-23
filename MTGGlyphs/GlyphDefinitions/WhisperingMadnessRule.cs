namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Whispering Madness, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Whispering Madness</exampledoc>
/// <examplecapture>each player discards their hand, then draws cards equal to the greatest number of cards a player discarded this way</examplecapture>
[TokenizationOrder(-1)]
public class WhisperingMadnessRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["each player discards their hand, then draws cards equal to the greatest number of cards a player discarded this way"];
}
