namespace MTGPlexer;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using MTGPlexer.CommonDTOs.StructuredMatches; // For StructuredTokenRoot
// Assuming TokenTypeRegistry is accessible via its namespace
// using MTGPlexer.SomeNamespace; 

public class CustomTokenizer
{
    private readonly List<Type> _orderedTokenTypes;
    private readonly Dictionary<Type, Regex> _anchoredRegexCache = [];
    private readonly Regex _whitespaceRegex = new(@"\G\s+", RegexOptions.Compiled);

    /// <summary>
    /// Initializes a new instance of the CustomTokenizer.
    /// </summary>
    /// <param name="orderedTokenTypes">A list of token Types, sorted by matching priority.</param>
    public CustomTokenizer(List<Type> orderedTokenTypes)
    {
        _orderedTokenTypes = orderedTokenTypes;

        // For performance, create anchored versions of the regexes from the registry.
        // The \G anchor ensures the regex only matches at the exact position
        // where the previous match ended, which is crucial for a tokenizer.
        foreach (var type in orderedTokenTypes)
        {
            if (TokenTypeRegistry.Templates.TryGetValue(type, out var template) && template.Regex != null)
            {
                var pattern = template.Regex.ToString();
                _anchoredRegexCache[type] = new Regex($"\\G({pattern})", RegexOptions.Compiled);
            }
        }
    }

    /// <summary>
    /// Tokenizes the input string into a sequence of StructuredTokenRoot objects.
    /// </summary>
    /// <param name="sourceText">The string to tokenize.</param>
    /// <returns>A list of tokens, including coalesced unmatched segments.</returns>
    public List<StructuredTokenRoot> Tokenize(string sourceText)
    {
        var tokens = new List<StructuredTokenRoot>();
        int currentIndex = 0;
        int unmatchedStartIndex = -1;

        while (currentIndex < sourceText.Length)
        {
            // 1. Skip all whitespace from the current position
            var spaceMatch = _whitespaceRegex.Match(sourceText, currentIndex);
            if (spaceMatch.Success)
            {
                FlushUnmatched(sourceText, tokens, ref unmatchedStartIndex, currentIndex);
                currentIndex += spaceMatch.Length;
                continue;
            }

            // 2. Attempt to match a known token in order of priority
            bool matched = false;
            foreach (var tokenType in _orderedTokenTypes)
            {
                if (_anchoredRegexCache.TryGetValue(tokenType, out var anchoredRegex))
                {
                    var match = anchoredRegex.Match(sourceText, currentIndex);
                    if (match.Success && match.Length > 0)
                    {
                        // Found a match. First, process any preceding unmatched text.
                        FlushUnmatched(sourceText, tokens, ref unmatchedStartIndex, currentIndex);

                        // Add the matched token.
                        var token = new StructuredTokenRoot(tokenType, sourceText, currentIndex, currentIndex + match.Length);
                        tokens.Add(token);

                        currentIndex += match.Length;
                        matched = true;
                        break; // Move to the next position in the source string
                    }
                }
            }

            // 3. If no token matched, handle as part of an unmatched sequence
            if (!matched)
            {
                if (unmatchedStartIndex == -1)
                {
                    unmatchedStartIndex = currentIndex;
                }
                currentIndex++;
            }
        }

        // Flush any remaining unmatched text at the end of the string
        FlushUnmatched(sourceText, tokens, ref unmatchedStartIndex, currentIndex);

        return tokens;
    }

    /// <summary>
    /// Helper method to create a single token from a sequence of unmatched characters.
    /// </summary>
    private void FlushUnmatched(string sourceText, List<StructuredTokenRoot> tokens, ref int unmatchedStartIndex, int currentIndex)
    {
        if (unmatchedStartIndex != -1)
        {
            int length = currentIndex - unmatchedStartIndex;
            if (length > 0)
            {
                var token = new StructuredTokenRoot(typeof(DefaultUnmatchedString), sourceText, unmatchedStartIndex, currentIndex);
                tokens.Add(token);
            }
        }
        unmatchedStartIndex = -1; // Reset the buffer
    }
}