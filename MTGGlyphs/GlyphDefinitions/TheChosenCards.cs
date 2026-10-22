namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"the chosen card(s)": cards picked earlier in the same ability, sent somewhere as one of a split.</summary>
/// <exampledoc>Fact or Fiction</exampledoc>
/// <examplecapture>the chosen cards</examplecapture>
[Dependent]
public class TheChosenCards : Glyph
{
    public override Nib[] Nibs => ["the chosen", Plural("card")];
}
