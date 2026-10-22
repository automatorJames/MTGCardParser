namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Either of two game states, as one condition: "a nonland permanent left the battlefield this turn or a spell was warped this turn" (void), "you attacked with a hero this turn or a hero entered the battlefield under your control this turn".</summary>
/// <exampledoc>Elegy Acolyte</exampledoc>
/// <examplecapture>a nonland permanent left the battlefield this turn or a spell was warped this turn</examplecapture>
[Dependent]
public class GameStateEither : Glyph
{
    public override Nib[] Nibs => [Prop(First), "or", Prop(Second)];

    public GameState First { get; set; }
    public GameState Second { get; set; }
}
