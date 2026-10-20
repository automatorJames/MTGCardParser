namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Taking control of a permanent, often for as long as a condition holds: "gain control of target artifact for as long as you control {this}". Also the second of two actions.</summary>
/// <exampledoc>Aladdin</exampledoc>
/// <examplecapture>gain control of target artifact for as long as you control {this}</examplecapture>
public class GainControl : Glyph, IConjoinedPredicate, IPredicate
{
    public override Nib[] Nibs => [Pattern("gains? control of"), Prop(Permanent), Prop(Duration)];

    public PermanentPhrase Permanent { get; set; }
    [Optional]
    public EffectDuration Duration { get; set; }
}
