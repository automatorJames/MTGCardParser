using MTGCardParser.TokenUnits.Interfaces;

namespace MTGCardParser.TokenUnits;

public class This : ITokenUnit
{
    public RegexTemplate<This> RegexTemplate => new(@"\{this\}");
}