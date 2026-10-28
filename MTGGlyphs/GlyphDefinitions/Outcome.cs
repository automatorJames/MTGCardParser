namespace MTGGlyphs.GlyphDefinitions;

/// <summary>How a choice, a requirement or a coin flip came out, as the condition of what follows it: "you do", "the player does", "you don't", "they can't", "you win the flip", or a named optional action not taken: "you don't cast it", "you didn't put the revealed card onto the battlefield this way". The condition of "if you do, …" and "if you don't, …" (see <see cref="ConditionalEffect"/>).</summary>
/// <exampledoc>Crystal Rod</exampledoc>
/// <examplecapture>you do</examplecapture>
[Dependent]
public class Outcome : Glyph
{
    public override Nib[] Nibs => [Prop(Player), Prop(Result)];

    public OneOf<ThatPlayer, TheyPlayer, WhichPlayer?> Player { get; set; }
    public OneOf<DidNotDo, DidNotDoThisWay, OutcomeResult?> Result { get; set; }
}
