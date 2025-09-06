namespace MTGPlexer.RegexSegmentDTOs;

public record AlternateValue
{
    public string Value { get; }
    public Regex Regex { get; }

    public AlternateValue(string value)
    {
        Value = value;
        Regex = new Regex(value, RegexOptions.Compiled);
    }
}

