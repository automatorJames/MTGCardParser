namespace MTGGlyphs.GlyphDefinitions;

/// <summary>How long, or while what, an effect applies: "until end of turn", "as long as you control a forest", "for as long as {this} remains tapped".</summary>
/// <exampledoc>Kird Ape</exampledoc>
/// <examplecapture>as long as you control a forest</examplecapture>
[Dependent]
public class EffectDuration : Glyph
{
    public override Nib[] Nibs => [Prop(Span)];

    public OneOf<UntilPhase, ControlCondition, ForAsLongAs> Span { get; set; }
}
