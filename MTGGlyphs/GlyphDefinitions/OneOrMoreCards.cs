namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"one or more cards", "one or more land cards", "one or more artifact and/or creature cards": a batch of cards a trigger watches as one event.</summary>
/// <exampledoc>Willow Geist</exampledoc>
/// <examplecapture>one or more cards</examplecapture>
[Dependent]
public class OneOrMoreCards : Glyph
{
    public override Nib[] Nibs => ["one or more", Prop(Qualifiers), "cards"];

    [Optional]
    public OneOf<CompoundOf<TargetQualifier>, QualifierWords> Qualifiers { get; set; }
}
