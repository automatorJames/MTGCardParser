namespace MTGPlexer.TokenUnits;

[NoSpaces]
public class GainOrLoseAbility : TokenUnit
{
    public GainOrLoseAbility() : base(nameof(LoseOrGain), " \"", nameof(Ability), "\"") { }

    public GainOrLose LoseOrGain { get; set; }

    [RegexPattern("[^\"]+")]
    public PlaceholderCapture Ability { get; set; }
}