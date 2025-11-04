namespace MTGPlexer.TokenUnits;

[NoSpaces]
public class GainOrLoseAbility : TokenUnit
{
    protected override string[] Snippets => [nameof(LoseOrGain), " \"", nameof(Ability), "\""];

    public GainOrLose LoseOrGain { get; set; }

    [RegexPattern("[^\"]+")]
    public PlaceholderCapture Ability { get; set; }
}