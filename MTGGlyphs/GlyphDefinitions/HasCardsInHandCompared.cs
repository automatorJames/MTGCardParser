namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"has more cards in hand": a player's hand size, compared.</summary>
/// <exampledoc>Oath of Scholars</exampledoc>
/// <examplecapture>has more cards in hand</examplecapture>
[Dependent]
public class HasCardsInHandCompared : Glyph
{
    public override Nib[] Nibs => [Prop(Verb), Prop(Comparison), "cards in", Opt(Pattern("(your|their)")), "hand"];

    public HaveVerb Verb { get; set; }
    public MoreOrFewer Comparison { get; set; }
}
