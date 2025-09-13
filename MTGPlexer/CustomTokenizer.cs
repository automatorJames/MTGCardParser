using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using MTGPlexer.CommonDTOs.StructuredMatches;
// using MTGPlexer.SomeNamespace; // For TokenTypeRegistry

public class CustomTokenizer
{
    private readonly List<Type> _orderedTokenTypes;
    private readonly Dictionary<Type, Regex> _anchoredRegexCache = [];

    // This regex helps efficiently find the start of the next potential token (word).
    private readonly Regex _nextWordStartRegex = new(@"\S+", RegexOptions.Compiled);

    public CustomTokenizer(List<Type> orderedTokenTypes)
    {
        _orderedTokenTypes = orderedTokenTypes;
        foreach (var type in orderedTokenTypes)
        {
            if (TokenTypeRegistry.Templates.TryGetValue(type, out var template) && template.Regex != null)
            {
                var pattern = template.Regex.ToString();
                _anchoredRegexCache[type] = new Regex($"\\G({pattern})", RegexOptions.Compiled);
            }
        }
    }

    public List<StructuredTokenRoot> Tokenize(string sourceText)
    {
        var tokens = new List<StructuredTokenRoot>();
        int currentIndex = 0;
        int unmatchedStartIndex = -1;

        while (currentIndex < sourceText.Length)
        {
            // First, consume any leading whitespace. This simplifies the logic
            // and ensures we always start evaluations on a non-whitespace character.
            if (char.IsWhiteSpace(sourceText[currentIndex]))
            {
                // Since we found whitespace, it definitively ends any preceding
                // unmatched block. Flush it now.
                FlushUnmatched(sourceText, tokens, ref unmatchedStartIndex, currentIndex);
                currentIndex++;
                continue;
            }

            bool matched = false;
            foreach (var tokenType in _orderedTokenTypes)
            {
                if (!_anchoredRegexCache.TryGetValue(tokenType, out var anchoredRegex))
                {
                    continue;
                }

                var match = anchoredRegex.Match(sourceText, currentIndex);
                if (match.Success && match.Length > 0)
                {
                    // A real token was found. Flush any pending unmatched text before adding it.
                    FlushUnmatched(sourceText, tokens, ref unmatchedStartIndex, currentIndex);

                    var token = new StructuredTokenRoot(tokenType, sourceText, currentIndex, currentIndex + match.Length);
                    tokens.Add(token);

                    currentIndex += match.Length;
                    matched = true;
                    break;
                }
            }

            if (!matched)
            {
                // No token matched. We are in an unmatched block.
                if (unmatchedStartIndex == -1)
                {
                    unmatchedStartIndex = currentIndex;
                }

                // Optimization: Instead of moving char by char, jump to the start of the next word.
                var nextWordMatch = _nextWordStartRegex.Match(sourceText, currentIndex + 1);
                currentIndex = nextWordMatch.Success ? nextWordMatch.Index : sourceText.Length;
            }
        }

        // The loop is finished. Flush any final unmatched text.
        FlushUnmatched(sourceText, tokens, ref unmatchedStartIndex, currentIndex);

        return tokens;
    }

    /// <summary>
    /// Creates a single, trimmed token for an entire block of unmatched text.
    /// </summary>
    private void FlushUnmatched(string sourceText, List<StructuredTokenRoot> tokens, ref int unmatchedStartIndex, int currentIndex)
    {
        if (unmatchedStartIndex == -1)
        {
            return; // Nothing to flush.
        }

        // --- BOUNDARY TRIMMING LOGIC ---
        int trimmedStart = unmatchedStartIndex;
        int trimmedEnd = currentIndex;

        // Move the start forward past any whitespace.
        while (trimmedStart < trimmedEnd && char.IsWhiteSpace(sourceText[trimmedStart]))
        {
            trimmedStart++;
        }

        // Move the end backward past any whitespace.
        while (trimmedEnd > trimmedStart && char.IsWhiteSpace(sourceText[trimmedEnd - 1]))
        {
            trimmedEnd--;
        }
        // --- END TRIMMING LOGIC ---

        // If the trimmed block has actual content, create the token.
        if (trimmedStart < trimmedEnd)
        {
            var token = new StructuredTokenRoot(typeof(DefaultUnmatchedString), sourceText, trimmedStart, trimmedEnd);
            tokens.Add(token);
        }

        // Reset the buffer.
        unmatchedStartIndex = -1;
    }
}