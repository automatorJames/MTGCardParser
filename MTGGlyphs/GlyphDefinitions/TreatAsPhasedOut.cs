namespace MTGGlyphs.GlyphDefinitions;

/// <summary>The newer wording of the phasing reminder: "treat it and anything attached to it as though they don't exist until its controller's next turn", "treat them and anything attached to them as though they don't exist" - a phased-out permanent, and what's attached to it, are treated as nonexistent, optionally until a given turn. The older wording is <see cref="PhasedOutRule"/>.</summary>
/// <exampledoc>Slip Out the Back</exampledoc>
/// <examplecapture>treat it and anything attached to it as though they don't exist until its controller's next turn</examplecapture>
public class TreatAsPhasedOut : Glyph
{
    public override Nib[] Nibs => ["treat", Prop(Phased), "and anything attached to", Alt("it", "them"), "as though they don't exist", Prop(Duration)];

    public OneOf<It, Them> Phased { get; set; }
    [Optional]
    public EffectDuration Duration { get; set; }
}
