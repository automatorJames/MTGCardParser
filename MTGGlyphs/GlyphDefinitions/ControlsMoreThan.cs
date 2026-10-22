namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A player controlling more or fewer permanents of a kind than another player, as a condition: "an opponent controls more lands than you", "that player controls more lands than each other player", "you control fewer creatures than each opponent".</summary>
/// <exampledoc>Land Tax</exampledoc>
/// <examplecapture>an opponent controls more lands than you</examplecapture>
[Dependent]
public class ControlsMoreThan : Glyph
{
    public override Nib[] Nibs => [Prop(Controller), Pattern("controls?"), Prop(Bound), Prop(Permanents), "than", Prop(Compared), Prop(Do)];

    public OneOf<WhichPlayer?, ThatPlayer> Controller { get; set; }
    public CountBound Bound { get; set; }
    public Permanents Permanents { get; set; }
    public OneOf<WhichPlayer?, ThatPlayer> Compared { get; set; }
    [Optional]
    [RegexPattern("do")]
    public bool? Do { get; set; }
}
