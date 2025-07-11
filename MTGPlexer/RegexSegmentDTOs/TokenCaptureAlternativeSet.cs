namespace MTGPlexer.RegexSegmentDTOs;

/// <summary>
/// This record contains a group of two or more alternative TokenRegexProps. It is expected that exactly
/// one of these alterantives will be matched within the Regex rendered for the associated TokenUnit, which means
/// all but one of the alternative child TokenUnit properties on the hydrated parent TokenUnit is expected to be null.
/// The record is related to the AlternativTokenUnits record in the sense that, during the construction of a RegexTemplate,
/// property names in each AlternativTokenUnits object are validated to exist as child TokenUnit properties, then are
/// each bound to a TokenRegexProp in the list of alternatives passed to this record.
/// </summary>
public record TokenCaptureAlternativeSet : RegexSegmentBase
{
    public List<TokenRegexProp> Alternatives { get; init; }

    public TokenCaptureAlternativeSet(List<TokenRegexProp> alternatives)
    {
        Alternatives = alternatives;
        SetRegex();
    }

    void SetRegex()
    {
        RegexString = "(";

        for (int i = 0; i < Alternatives.Count; i++)
        {
            RegexString += Alternatives[i].RegexString;

            if (i < Alternatives.Count - 1)
                RegexString += "|";
        }

        RegexString += ")";
        Regex = new Regex(RegexString);
    }
}
