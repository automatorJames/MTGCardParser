namespace MTGPlexer.TokenAnalysis.RegexDTOs;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

public record TokenUnitCapture
{
    public Type Type { get; }
    public int OccurrenceCount { get; }
    public string RegexString { get; }
    public PrettifiedRegex PrettifiedRegex { get; } // Changed from string to the new record type
    public List<RegexPropValueSet> RegexPropValueSets { get; } = [];
    public DeterministicPalette Palette { get; }

    public TokenUnitCapture(Type type, int occurrenceCount, Dictionary<TerminalRegexPropPath, Dictionary<string, ValueCaptureVariantCollector>> collectors = null)
    {
        Type = type;
        OccurrenceCount = occurrenceCount;
        Palette = TokenTypeRegistry.Palettes[type];
        RegexString = TokenTypeRegistry.Templates[type].RenderedRegexString;

        if (collectors != null)
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

        // The only responsibility now is to call the factory method.
        PrettifiedRegex = PrettifiedRegex.Create(RegexString);
    }

    // This method remains as it's a useful utility for your other logic.
    (int start, int endExclusive) FindNamedCaptureGroupSpan(string name)
    {
        var regex = new Regex(
            $@"\(\?\<{Regex.Escape(name)}\>.*?\)",
            RegexOptions.Singleline);
        var match = regex.Match(RegexString);
        return match.Success ? (match.Index, match.Index + match.Length) : (-1, -1);
    }
}