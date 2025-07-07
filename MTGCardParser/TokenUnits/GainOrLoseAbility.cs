namespace MTGCardParser.TokenUnits;

[NoSpaces]
public class GainOrLoseAbility : TokenUnit
{
    public RegexTemplate<GainOrLoseAbility> RegexTemplate => new(nameof(LoseOrGain), " \"", nameof(Ability), "\"");


    [RegexPattern("[^\"]+")]
    public CapturedTextSegment Ability { get; set; }

    public LoseOrGain? LoseOrGain { get; set; }
}