namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Life or damage a player changed: "you lost", "they gained", "your opponents have lost this turn", "they paid this way".</summary>
/// <exampledoc>Transcendence</exampledoc>
/// <examplecapture>you lost</examplecapture>
[Dependent]
public class PlayerChangedLife : Glyph
{
    public override Nib[] Nibs => [Prop(Player), Opt("have"), Prop(Change), Prop(When)];

    public OneOf<WhichPlayer?, They> Player { get; set; }
    public DamageOrLifeChange Change { get; set; }
    [Optional]
    public DamageOrLifeWhen? When { get; set; }
}
