namespace MTGGlyphs.GlyphDefinitions;

/// <summary>The cards left over from a reveal or exile loop: "all other cards revealed this way", "the other cards exiled this way". A group named by what happened to it earlier in the line.</summary>
/// <exampledoc>Polymorph</exampledoc>
/// <examplecapture>all other cards revealed this way</examplecapture>
[Dependent]
public class OtherCardsRevealedThisWay : Glyph
{
    public override Nib[] Nibs => [Alt("all other", "the other"), Plural("card"), Alt("revealed", "exiled"), "this way"];
}
