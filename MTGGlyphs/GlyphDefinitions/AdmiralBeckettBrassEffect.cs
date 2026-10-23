namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Admiral Beckett Brass, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Admiral Beckett Brass</exampledoc>
/// <examplecapture>gain control of target nonland permanent controlled by a player who was dealt combat damage by three or more pirates this turn</examplecapture>
[Dependent]
public class AdmiralBeckettBrassEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["gain control of target nonland permanent controlled by a player who was dealt combat damage by three or more pirates this turn"];
}
