namespace MTGCardParser.TokenCaptures;

public class LoseOrGainAbility : TokenCaptureBase<LoseOrGainAbility>
{
    public override RegexTemplate<LoseOrGainAbility> RegexTemplate => new(nameof(LoseOrGain), "\"", nameof(Ability), "\"");


    [RegexPattern("[^\"]+")]
    public TokenSegment Ability { get; set; }

    public LoseOrGain? LoseOrGain { get; set; }
}