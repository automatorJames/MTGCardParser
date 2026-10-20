namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"one or two targets", "one, two, or three target attacking or blocking creatures": a choice among small numbers of targets.</summary>
/// <exampledoc>Arc Lightning</exampledoc>
/// <examplecapture>one, two, or three targets</examplecapture>
[Dependent]
public class ChosenNumberOfTargets : Glyph
{
    public override Nib[] Nibs => [Prop(Counts), Prop(Targets)];

    public ManyOf<Quantity> Counts { get; set; }
    public OneOf<TargetsOfKind, DividedRecipients?> Targets { get; set; }
}
