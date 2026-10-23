namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Teferi's Puzzle Box, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Teferi's Puzzle Box</exampledoc>
/// <examplecapture>that player puts the cards in their hand on the bottom of their library in any order, then draws that many cards</examplecapture>
[Dependent]
public class TeferisPuzzleBoxEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["that player puts the cards in their hand on the bottom of their library in any order, then draws that many cards"];
}
