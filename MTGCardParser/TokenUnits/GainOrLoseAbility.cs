namespace MTGCardParser.TokenUnits;

[NoSpaces]
public class GainOrLoseAbility : TokenUnit
{
    public GainOrLoseAbility() : base(nameof(LoseOrGain), " \"", nameof(Ability), "\"") { }

    public LoseOrGain? LoseOrGain { get; set; }

    [RegexPattern("[^\"]+")]
    public PlaceholderCapture Ability { get; set; }
}