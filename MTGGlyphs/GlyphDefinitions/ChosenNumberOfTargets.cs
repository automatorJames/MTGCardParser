namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"one or two targets", "one, two, or three target attacking or blocking creatures", "x target creatures and/or planeswalkers": a choice among small numbers of targets, or a set number of them.</summary>
/// <exampledoc>Arc Lightning</exampledoc>
/// <examplecapture>one, two, or three targets</examplecapture>
[Dependent]
public class ChosenNumberOfTargets : Glyph
{
    public override Nib[] Nibs => [Prop(Counts), Prop(Targets)];

    public OneOf<ManyOf<Quantity>, Quantity?> Counts { get; set; }
    public OneOf<TargetsOfKind, DividedRecipients?> Targets { get; set; }
}
