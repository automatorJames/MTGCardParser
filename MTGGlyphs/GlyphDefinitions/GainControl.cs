namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Taking control of a permanent or group, often for as long as a condition holds: "gain control of target artifact for as long as you control {this}", "gain control of all permanents you own", "gain control of any number of target creatures until end of turn", "gain control of a land you don't control". Also the second of two actions.</summary>
/// <exampledoc>Aladdin</exampledoc>
/// <examplecapture>gain control of target artifact for as long as you control {this}</examplecapture>
public class GainControl : Glyph, IConjoinedPredicate, IPredicate
{
    public override Nib[] Nibs => [Pattern("gains? control of"), Prop(Permanent), Prop(Duration)];

    public OneOf<PermanentPhrase, OneOf<AllPermanents, CountedTargets>, IndefinitePermanent> Permanent { get; set; }
    [Optional]
    public EffectDuration Duration { get; set; }
}
