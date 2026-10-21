namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A batch of permanents a trigger watches as one event: "one or more creatures you control", "one or more nontoken creatures you control", "one or more other elves you control", "one or more tokens your opponents control". The kind is a referent, for a later "those creatures".</summary>
/// <exampledoc>Ongoing Investigation</exampledoc>
/// <examplecapture>one or more creatures you control</examplecapture>
[Dependent]
public class OneOrMorePermanents : Glyph
{
    public override Nib[] Nibs => ["one or more", Prop(Other), Prop(Permanents)];

    [RegexPattern("other")]
    public bool Other { get; set; }
    public Permanents Permanents { get; set; }
}
