namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A restriction on when an ability may be activated, or how often it triggers: "activate only during your upkeep", "activate only during your turn and only once each turn", "activate only as a sorcery", "equip only as a sorcery", "saddle only as a sorcery", "this ability triggers only once each turn".</summary>
/// <exampledoc>Colossus of Sardia</exampledoc>
/// <examplecapture>activate only during your upkeep</examplecapture>
public class ActivationRestriction : Glyph
{
    public override Nib[] Nibs => [Alt("activate", "equip", "saddle", "this ability triggers"), "only", Prop(Timings)];

    public OneOf<ManyOf<ActivationTiming>, ActivationTiming> Timings { get; set; }
}
