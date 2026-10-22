namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A player's life total against another player's, as a condition: "an opponent has more life than you", "you have less life than an opponent", "a player has more life than each other player".</summary>
/// <exampledoc>Sunset Revelry</exampledoc>
/// <examplecapture>an opponent has more life than you</examplecapture>
[Dependent]
public class LifeComparedTo : Glyph
{
    public override Nib[] Nibs => [Prop(Player), Prop(Verb), Prop(Comparison), "life than", Prop(Compared), Prop(Do)];

    public OneOf<WhichPlayer?, ThatPlayer> Player { get; set; }
    public HaveVerb Verb { get; set; }
    public Comparison Comparison { get; set; }
    public OneOf<WhichPlayer?, ThatPlayer> Compared { get; set; }
    [Optional]
    [RegexPattern("do")]
    public bool? Do { get; set; }
}
