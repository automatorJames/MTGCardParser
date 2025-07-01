namespace MTGCardParser.RegexSegmentDTOs;

public record CaptureAlternatives
{
    public string[] Names;

    public CaptureAlternatives(params string[] names)
    {
        if (names.Length < 2)
            throw new ArgumentException("At least 2 capture alternatives required");

        Names = names;
    }
}

