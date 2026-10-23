namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Possibility Storm, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Possibility Storm</exampledoc>
/// <examplecapture>that player exiles it, then exiles cards from the top of their library until they exile a card that shares a card type with it</examplecapture>
[Dependent]
public class PossibilityStormEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["that player exiles it, then exiles cards from the top of their library until they exile a card that shares a card type with it"];
}
