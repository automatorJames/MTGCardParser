using MTGCardParser.TokenUnits.Interfaces;

namespace MTGCardParser.TokenUnits;

public class IfYouDo : ITokenUnit
{
    public RegexTemplate<IfYouDo> RegexTemplate => new("if you do,");
}