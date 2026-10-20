namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Learn's reminder text.</summary>
/// <exampledoc>Rise of Extus</exampledoc>
/// <examplecapture>you may reveal a lesson card you own from outside the game and put it into your hand, or discard a card to draw a card</examplecapture>
public class LearnRule : Glyph
{
    public override Nib[] Nibs => ["you may reveal a lesson card you own from outside the game and put it into your hand, or discard a card to draw a card"];
}
