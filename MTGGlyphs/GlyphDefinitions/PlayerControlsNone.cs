namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"you control no untapped lands", "defending player controls no creatures", "you control no other creatures", "its controller controls no other creatures" (a player named earlier): a player controlling no permanents of a kind (optionally besides this one), as a condition.</summary>
/// <exampledoc>Scoria Cat</exampledoc>
/// <examplecapture>you control no untapped lands</examplecapture>
[Dependent]
public class PlayerControlsNone : Glyph
{
    public override Nib[] Nibs => [Prop(Controller), Pattern("controls?"), "no", Prop(Permanents)];

    public OneOf<WhichPlayer?, PlayerReference> Controller { get; set; }
    public OneOf<OtherPermanents, Permanents> Permanents { get; set; }
}
