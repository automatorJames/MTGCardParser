namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Condition on how a spell was cast: "{this} was kicked" / "it was kicked" / "this spell was kicked", optionally naming which kicker ("it was kicked with its {2}{u} kicker"); "this spell was bargained"; "this spell was cast using teamwork".</summary>
/// <exampledoc>Caustic Tar</exampledoc>
/// <examplecapture>{this} was kicked</examplecapture>
[Dependent]
public class WasKicked : Glyph
{
    public override Nib[] Nibs => [Prop(Subject), "was", Prop(How), Prop(Kicker)];

    public OneOf<This, It, ThisObject> Subject { get; set; }
    public OneOf<OptionalCostVerb?, CastUsingKeyword> How { get; set; }
    [Optional]
    public WithItsKicker Kicker { get; set; }
}
