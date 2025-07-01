using MTGCardParser.TokenUnits.Interfaces;

namespace MTGCardParser.TokenUnits;

[NoSpaces]
public class LoseOrGainAbility : ITokenUnit
{
    public RegexTemplate<LoseOrGainAbility> RegexTemplate => new(nameof(LoseOrGain), " \"", nameof(Ability), "\"");


    [RegexPattern("[^\"]+")]
    public CapturedTextSegment Ability { get; set; }

    public LoseOrGain? LoseOrGain { get; set; }
}