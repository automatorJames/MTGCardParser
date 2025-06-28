namespace MTGCardParser.TokenCaptures;

[NoSpaces]
public class LoseOrGainAbility : ITokenCapture
{
    public RegexTemplate<LoseOrGainAbility> RegexTemplate => new(nameof(LoseOrGain), " \"", nameof(Ability), "\"");


    [RegexPattern("[^\"]+")]
    public TokenSegment Ability { get; set; }

    public LoseOrGain? LoseOrGain { get; set; }
}