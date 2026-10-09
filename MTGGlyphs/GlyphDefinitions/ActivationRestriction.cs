namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A restriction on when an activated ability may be activated: "activate only during your upkeep", "activate only during your turn and only once each turn", "activate only as a sorcery".</summary>
/// <exampledoc>Colossus of Sardia</exampledoc>
/// <examplecapture>activate only during your upkeep</examplecapture>
public class ActivationRestriction : Glyph
{
    public override Nib[] Nibs => ["activate only", Prop(Timings)];

    public OneOf<ManyOf<ActivationTiming>, ActivationTiming> Timings { get; set; }
}
