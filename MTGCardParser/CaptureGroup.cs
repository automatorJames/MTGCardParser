
using System.Text;

namespace MTGCardParser;

public record CaptureGroup : IRegexSegment
{
    public string Name { get; init; }
    public string RegexString { get; private set; }
    public IEnumerable<string> Alternatives { get; init; }
    public CapturePropType CapturePropType { get; init; }
    public RegexOptionsAttribute Options { get; init; }
    public Regex Regex { get; private set; }


    public CaptureGroup(
        string name,
        IEnumerable<string> alternatives,
        CapturePropType capturePropType,
        RegexOptionsAttribute options)
    {
        Alternatives = alternatives;
        Name = name;
        CapturePropType = capturePropType;
        Options = options;
        SetRegexPatterns();
    }

    void SetRegexPatterns()
    {
        var items = Alternatives.OrderByDescending(s => s.Length).ToList();

        if (Options.OptionalPlural)
            for (int i = 0; i < items.Count; i++)
            {
                string word = items[i];
                word = IRegexSegment.AddOptionalPluralization(word);
            }

        var combinedItems = string.Join('|', items);
        var isBool = CapturePropType == CapturePropType.Bool;
        RegexString = $"(?<{Name}>{(isBool ? @"\s?" : "")}{combinedItems}{(isBool ? @"\s?" : "")}){(isBool ? "?" : "")}";
        Regex = new Regex(RegexString);
    }
}

