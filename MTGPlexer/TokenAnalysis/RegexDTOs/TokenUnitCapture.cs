namespace MTGPlexer.TokenAnalysis.RegexDTOs;

public record TokenUnitCapture
{
    public Type Type { get; }
    public int OccurrenceCount { get; }
    public string RegexString { get; }
    public List<RegexPropValueSet> RegexPropValueSets { get; } = [];
    public DeterministicPalette Palette { get; }

    public TokenUnitCapture(Type type, int occurrenceCount, Dictionary<string, Dictionary<string, ValueCaptureVariantCollector>> collectors = null)
    {
        Type = type;
        OccurrenceCount = occurrenceCount;
        Palette = TokenTypeRegistry.Palettes[type];
        RegexString = TokenTypeRegistry.Templates[type].RenderedRegexString;

        if (collectors is null)
        {
            RegexPropValueSets = [];
            return;
        }

        foreach (var propPathValSetCollector in collectors)
        {
            var variantSets = propPathValSetCollector.Value.Values
                .Select(x => new ValueCaptureVariantSet(x, RegexString, propPathValSetCollector.Key))
                .ToList();

            var (captureGroupStart, captureGroupEnd) = FindNamedCaptureGroupSpan(propPathValSetCollector.Key);
            RegexPropValueSets.Add(new RegexPropValueSet(propPathValSetCollector.Key, captureGroupStart, captureGroupEnd, variantSets));
        }
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
}

