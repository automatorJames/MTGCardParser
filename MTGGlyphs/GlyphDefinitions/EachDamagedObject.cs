namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"each creature dealt damage this way", "each creature dealt damage this turn": every object of a kind that was dealt damage, by the effect just described or earlier this turn.</summary>
/// <exampledoc>Inflame</exampledoc>
/// <examplecapture>each creature dealt damage this turn</examplecapture>
[Dependent]
public class EachDamagedObject : Glyph
{
    public override Nib[] Nibs => ["each", Prop(Kind), "dealt damage", Prop(When)];

    public TargetKind Kind { get; set; }
    public OneOf<ThisWay, ThisPeriod> When { get; set; }
}
