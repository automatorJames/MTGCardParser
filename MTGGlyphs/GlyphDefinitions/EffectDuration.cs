namespace MTGGlyphs.GlyphDefinitions;

/// <summary>How long, or while what, an effect applies: "until end of turn", "as long as you control a forest". A glyph wrapping a one-of, so it can be optional.</summary>
/// <exampledoc>Kird Ape</exampledoc>
/// <examplecapture>as long as you control a forest</examplecapture>
[Dependent]
public class EffectDuration : Glyph
{
    public override Nib[] Nibs => [Prop(Span)];

    public OneOf<UntilPhase, ControlCondition> Span { get; set; }
}
