namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A number of permanents of one kind: "two islands", "four creatures", "two lands", "any number of creatures" - as sacrificed or chosen.</summary>
/// <exampledoc>Psychic Allergy</exampledoc>
/// <examplecapture>two islands</examplecapture>
[Dependent]
public class CountedPermanents : Glyph
{
    public override Nib[] Nibs => [Prop(Count), Opt("of"), Prop(Permanents)];

    public Quantity Count { get; set; }
    public Permanents Permanents { get; set; }
}
