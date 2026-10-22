namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Condition on how a spell was cast: "{this} was kicked" / "it was kicked" / "this spell was kicked" / "that spell was kicked" / "he was kicked" / "grunn was kicked", or not ("this creature wasn't kicked"), optionally how often ("it was kicked twice") or naming which kicker ("it was kicked with its {2}{u} kicker"); "this spell was bargained"; "this spell was cast using teamwork".</summary>
/// <exampledoc>Caustic Tar</exampledoc>
/// <examplecapture>{this} was kicked</examplecapture>
[Dependent]
public class WasKicked : Glyph
{
    public override Nib[] Nibs => [Prop(Subject), Alt("was", "wasn't"), Prop(How), Prop(Twice), Prop(Kicker)];

    public OneOf<This, OneOf<It, PersonalPronoun, ThatCard>, OneOf<ThisObject, ShortName>> Subject { get; set; }
    public OneOf<OptionalCostVerb?, CastUsingKeyword> How { get; set; }
    [Optional]
    [RegexPattern("twice")]
    public bool Twice { get; set; }
    [Optional]
    public WithItsKicker Kicker { get; set; }
}
