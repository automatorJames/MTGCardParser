namespace MTGCardParser.TokenUnits;

[NoSpaces]
public class GainOrLoseAbility : TokenUnit
{
    public RegexTemplate<GainOrLoseAbility> RegexTemplate => new(nameof(LoseOrGain), " \"", nameof(Ability), "\"");

    public LoseOrGain? LoseOrGain { get; set; }

    [RegexPattern("[^\"]+")]
    public PlaceholderCapture Ability { get; set; }
}