namespace MTGCardParser.RegexSegmentDTOs;

/*public record TokenCaptureAlternativeSet : RegexSegmentBase
{
    public List<TokenCaptureSegment> Alternatives { get; init; }

    public TokenCaptureAlternativeSet(List<TokenCaptureSegment> alternatives)
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
}*/
