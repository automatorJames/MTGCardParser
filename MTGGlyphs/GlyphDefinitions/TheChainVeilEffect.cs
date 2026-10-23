namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of The Chain Veil, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>The Chain Veil</exampledoc>
/// <examplecapture>for each planeswalker you control, you may activate one of its loyalty abilities once this turn as though none of its loyalty abilities have been activated this turn</examplecapture>
[Dependent]
public class TheChainVeilEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["for each planeswalker you control, you may activate one of its loyalty abilities once this turn as though none of its loyalty abilities have been activated this turn"];
}
