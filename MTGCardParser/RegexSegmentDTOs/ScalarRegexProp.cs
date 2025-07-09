namespace MTGCardParser.RegexSegmentDTOs;

/// <summary>
/// Represents the subset of Regex properties with scalar values (i.e. bool & text placeholder). Note the term "scalar" here
/// does not refer to enum values, which is merely due to a difference in how enums are captured/patterned and "hydrated" from 
/// captures.
/// </summary>
public record ScalarRegexProp : RegexPropBase
{
    public ScalarRegexProp(RegexPropInfo captureProp) : base(captureProp)
    {
        SetRegex();
    }

    void SetRegex()
    {
        var items = RegexPropInfo.AttributePatterns.OrderByDescending(s => s.Length).ToList();
        var combinedItems = string.Join('|', items);
        var isBool = RegexPropInfo.CapturePropType == RegexPropType.Bool;
        RegexString = $"(?<{RegexPropInfo.Name}>{(isBool ? @"\s?" : "")}{combinedItems}{(isBool ? @"\s?" : "")}){(isBool ? "?" : "")}";
        Regex = new Regex(RegexString);
    }
}

