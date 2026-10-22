namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A number of permanents of one kind: "two islands", "four creatures", "two lands", "any number of creatures", "two other creatures" (the frame records "other", excluding this permanent), "up to two lands" (an upper bound) - as sacrificed, returned or chosen.</summary>
/// <exampledoc>Psychic Allergy</exampledoc>
/// <examplecapture>two islands</examplecapture>
[Dependent]
public class CountedPermanents : Glyph
{
    public override Nib[] Nibs => [Opt("up to"), Prop(Count), Opt("of"), Opt("other"), Prop(Permanents)];

    public Quantity Count { get; set; }
    public Permanents Permanents { get; set; }
}
