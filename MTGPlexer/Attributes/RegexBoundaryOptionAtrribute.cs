namespace MTGPlexer.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class RegexBoundaryOptionAtrribute : Attribute
{
    public BoundaryOption Option { get; set; }

    public RegexBoundaryOptionAtrribute(BoundaryOption option)
    {
        Option = option;
    }
}

public enum BoundaryOption
{
    None,
    OptionalTerminalPeriod,
    WholeWord,
    FullLine
}

