namespace MTGCardParser;

public record CaptureGroupTemplate
{
    public string[] Alternatives { get; init; }
    public string Name { get; init; }
    public CapturePropType CapturePropType { get; init; }
    public bool IsFirst { get; init; }
    public bool IsLast { get; init; }
    public RegexOptionsAttribute Options { get; init; }
    public string RegexPattern { get; init; }

    public CaptureGroupTemplate(
        string[] alternatives,
        string name,
        CapturePropType capturePropType,
        bool isFirst,
        bool isLast,
        RegexOptionsAttribute options)
    {
        Alternatives = alternatives;
        Name = name;
        CapturePropType = capturePropType;
        IsFirst = isFirst;
        IsLast = isLast;
        Options = options;
        RegexPattern = Stringify();
    }

    string Stringify()
    {
        var items = Alternatives.OrderByDescending(s => s.Length).ToList();

        if (Options.OptionalPlural)
            for (int i = 0; i < items.Count; i++)
            {
                string word = Alternatives[i];
                word = AddOptionalPluralization(word);
            }

        var combinedItems = string.Join('|', items);
        return EncloseInCaptureGroupWithSpacing(combinedItems);
    }

    string EncloseInCaptureGroupWithSpacing(string content)
    {
        if (CapturePropType == CapturePropType.Bool)
            return $@"(?<{Name}>{(IsFirst ? "" : " ")}{content}{(IsLast ? "" : " ")})";
        else
            return $@"{(IsFirst ? "" : " ")}(?<{Name}>{content}){(IsLast ? "" : " ")}";
    }

    string AddOptionalPluralization(string word)
    {
        if (string.IsNullOrWhiteSpace(word))
            throw new ArgumentNullException(nameof(word));

        if (word.EndsWith("y", StringComparison.OrdinalIgnoreCase) && word.Length > 1 && !"aeiou".Contains(char.ToLower(word[word.Length - 2])))
            word = word.Substring(0, word.Length - 1) + "(ies)";
        else if (word.EndsWith("s", StringComparison.OrdinalIgnoreCase) ||
            word.EndsWith("x", StringComparison.OrdinalIgnoreCase) ||
            word.EndsWith("z", StringComparison.OrdinalIgnoreCase) ||
            word.EndsWith("ch", StringComparison.OrdinalIgnoreCase) ||
            word.EndsWith("sh", StringComparison.OrdinalIgnoreCase))
            word += "(es)";
        else word += "(s)";

        return word + "?";
    }
}

