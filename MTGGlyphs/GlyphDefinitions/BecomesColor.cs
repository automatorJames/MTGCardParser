namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A color-changing effect: "target spell or permanent becomes red" (the Lace cycle), "one or more target creatures become white until end of turn".</summary>
/// <exampledoc>Chaoslace</exampledoc>
/// <examplecapture>target spell or permanent becomes red</examplecapture>
public class BecomesColor : Glyph
{
    public override Nib[] Nibs => [Prop(Changed), Pattern("becomes?"), Prop(Color), Prop(Duration)];

    public OneOf<TargetSpellOrPermanent, CountedTargets> Changed { get; set; }
    public ManaColor Color { get; set; }
    [Optional]
    public UntilPhase Duration { get; set; }
}
