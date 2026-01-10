namespace MTGPlexer.TokenUnits;

[NoSpaces]
public class GainOrLoseAbility : TokenUnit
{
    protected override Snippet[] Snippets => [Prop(LoseOrGain), " \"", Prop(Ability), "\""];

    public GainOrLose LoseOrGain { get; set; }

    [RegexPattern("[^\"]+")]
    public PlaceholderCapture Ability { get; set; }
}