namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Total War, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Total War</exampledoc>
/// <examplecapture>destroy all untapped non-wall creatures that player controls that didn't attack, except for creatures the player hasn't controlled continuously since the beginning of the turn</examplecapture>
[Dependent]
public class TotalWarEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["destroy all untapped non-wall creatures that player controls that didn't attack, except for creatures the player hasn't controlled continuously since the beginning of the turn"];
}
