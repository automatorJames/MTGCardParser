namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A counted number of cards of a kind, as a reveal-until loop's stop condition: "two land cards", "three nonland cards".</summary>
/// <exampledoc>Mind Funeral</exampledoc>
/// <examplecapture>four land cards</examplecapture>
[Dependent]
public class CountedCards : Glyph
{
    public override Nib[] Nibs => [Prop(Count), Prop(Qualifiers), "cards"];

    public Quantity Count { get; set; }
    [Optional]
    public OneOf<CompoundOf<TargetQualifier>, QualifierWords> Qualifiers { get; set; }
}
