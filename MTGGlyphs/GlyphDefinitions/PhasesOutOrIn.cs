namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Phasing a permanent out or in: "this creature phases out", "target artifact, creature, or land phases out", "all lands you control phase out", optionally for a while.</summary>
/// <exampledoc>Mist Dragon</exampledoc>
/// <examplecapture>phases out</examplecapture>
[Dependent]
public class PhasesOutOrIn : Glyph, IConjoinedPredicate, IPredicate
{
    public override Nib[] Nibs => [Pattern("phases?"), Prop(Direction), Prop(Duration)];

    public PhaseDirection Direction { get; set; }
    [Optional]
    public EffectDuration Duration { get; set; }
}
