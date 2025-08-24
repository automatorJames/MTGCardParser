namespace MTGPlexer.TokenAnalysis.RegexDTOs;

public record TokenUnitCapture
{
    // Regex to find named capture groups for prettification. Compiled for performance.
    private static readonly Regex PrettifyNamedGroupRegex = new Regex(
        @"\(\?<(?<name>\w+)>(?<content>[^)]+)\)(?<optional>\?)?",
        RegexOptions.Compiled);

    public Type Type { get; }
    public int OccurrenceCount { get; }
    public string RegexString { get; }
    public string PrettifiedRegexString { get; }
    //public PrettifiedRegex PrettifiedRegex { get; }
    public List<RegexPropValueSet> RegexPropValueSets { get; } = [];
    public DeterministicPalette Palette { get; }

    public TokenUnitCapture(Type type, int occurrenceCount, Dictionary<TerminalRegexPropPath, Dictionary<string, ValueCaptureVariantCollector>> collectors = null)
    {
        Type = type;
        OccurrenceCount = occurrenceCount;
        Palette = TokenTypeRegistry.Palettes[type];
        RegexString = TokenTypeRegistry.Templates[type].RenderedRegexString;

        if (collectors is null)
        {
            RegexPropValueSets = [];
        }
        else
        {
            foreach (var propPathValSetCollector in collectors)
            {
                var variantSets = propPathValSetCollector.Value.Values
                    .Select(x => new ValueCaptureVariantSet(x, RegexString, propPathValSetCollector.Key.TerminalPropName))
                    .ToList();

                var (captureGroupStart, captureGroupEnd) = FindNamedCaptureGroupSpan(propPathValSetCollector.Key.TerminalPropName);
                RegexPropValueSets.Add(new RegexPropValueSet(propPathValSetCollector.Key, captureGroupStart, captureGroupEnd, variantSets));
            }
        }

        //PrettifiedRegex = new(RegexString);

        PrettifiedRegexString = RegexString;
    }

    

    // Returns (start, endExclusive) of the named group "(?<name> ... )" within 'pattern',
    // or (-1, -1) if not found.
    (int start, int endExclusive) FindNamedCaptureGroupSpan(string name)
    {
        var regex = new Regex(
            $@"\(\?\<{Regex.Escape(name)}\>         # group start: (?<name>
                (?:                                  # body:
                    \[(?:\\.|[^\]\\])*\]            #   character class (skip)
                  | \\.
                  | \((?<DEPTH>)                    #   open paren -> push
                  | \)(?<-DEPTH>)                   #   close paren -> pop
                  | [^()[\\]+                       #   other chars
                )*
                (?(DEPTH)(?!))                      # depth must be zero here
                \)                                  # closing ) of the named group
            ",
            RegexOptions.IgnorePatternWhitespace | RegexOptions.Singleline);

        var match = regex.Match(RegexString);
        return match.Success ? (match.Index, match.Index + match.Length) : (-1, -1);
    }

    /// <summary>
    /// Converts the RegexString into a more readable, "prettified" format based on specific rules.
    /// </summary>
    /// <returns>A prettified, multi-line version of the regex.</returns>
    private string GetPrettifiedRegex()
    {
        if (string.IsNullOrEmpty(RegexString))
        {
            return string.Empty;
        }

        var sb = new StringBuilder();
        var matches = PrettifyNamedGroupRegex.Matches(RegexString);
        int lastIndex = 0;
        bool isFirstToken = true;

        foreach (Match match in matches)
        {
            // 1. Process text fragment BEFORE the current match
            string interveningText = RegexString.Substring(lastIndex, match.Index - lastIndex);
            if (!string.IsNullOrEmpty(interveningText))
            {
                if (!isFirstToken) sb.AppendLine();
                sb.Append(PrettifyInternalText(interveningText));
                isFirstToken = false;
            }

            // 2. Process the named capture group itself
            if (!isFirstToken) sb.AppendLine();
            PrettifyNamedGroup(sb, match);
            isFirstToken = false;

            lastIndex = match.Index + match.Length;
        }

        // 3. Process any text fragment remaining AFTER the last match
        string remainingText = RegexString.Substring(lastIndex);
        if (!string.IsNullOrEmpty(remainingText))
        {
            if (!isFirstToken) sb.AppendLine();
            sb.Append(PrettifyInternalText(remainingText));
        }

        return sb.ToString();
    }

    /// <summary>
    /// Formats a matched named capture group with indentation, line breaks, and comments.
    /// </summary>
    private static void PrettifyNamedGroup(StringBuilder sb, Match match)
    {
        const string indentation = "    ";
        string groupName = match.Groups["name"].Value;
        string content = match.Groups["content"].Value;
        bool isOptional = match.Groups["optional"].Success;

        sb.Append($"(?<{groupName}>");

        string[] alternatives = content.Split('|');

        // Append each alternative on an indented new line
        for (int i = 0; i < alternatives.Length; i++)
        {
            sb.AppendLine();
            sb.Append(indentation);
            if (i > 0)
            {
                // Rule 2: Always add a space after every "|"
                sb.Append("| ");
            }
            sb.Append(PrettifyInternalText(alternatives[i].Trim()));
        }

        sb.AppendLine();
        sb.Append(')');

        if (isOptional)
        {
            sb.Append("? #optional capture group");
        }
    }

    /// <summary>
    /// Prettifies a fragment by replacing spaces and \s.
    /// </summary>
    private static string PrettifyInternalText(string fragment)
    {
        fragment = Regex.Replace(fragment, @"(?<!\[) (?!\])", "[ ]");
        fragment.Replace(@"\s", "[ ]");

        return fragment;
    }
}