namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A hand size compared with another player's: "more cards in hand than each opponent", "fewer cards in hand than you", "more cards in their hand than you", "more cards in hand than they do".</summary>
/// <exampledoc>Death of a Thousand Stings</exampledoc>
/// <examplecapture>more cards in hand than each opponent</examplecapture>
[Dependent]
public class ComparedHandSize : Glyph
{
    public override Nib[] Nibs => [Prop(Comparison), "cards in", Opt(Pattern("(your|their)")), "hand than", Prop(Other), Prop(Do)];

    public MoreOrFewer Comparison { get; set; }
    public OneOf<WhichPlayer?, PlayerReference> Other { get; set; }
    [Optional]
    [RegexPattern("do")]
    public bool? Do { get; set; }
}
