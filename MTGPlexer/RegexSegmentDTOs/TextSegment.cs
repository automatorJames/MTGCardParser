using System.Text;

namespace MTGPlexer.RegexSegmentDTOs;

/// <summary>
/// Records a simple string Regex pattern, and applies context-aware ("smart") word boundaries as appropriate.
/// This record is used for strings defined in RegexTemplate expression bodies. These strings aren't associated
/// with any TokenUnit property, but rather must be matched as part of the TokenUnit's overall Regex.
/// </summary>
public record TextSegment : RegexSegmentBase
{
    public TextSegment(string pattern)
    {
        RegexString = WrapInSmartWordBoundaries(pattern);
        Regex = new Regex(RegexString);
    }

    public static string WrapInSmartWordBoundaries(string regexPattern)
    {
        // Return immediately for null or empty strings.
        if (string.IsNullOrEmpty(regexPattern))
            return regexPattern;

        bool prependBoundary = false;
        bool appendBoundary = false;

        // --- Check if the pattern STARTS with a word character ---

        char firstChar = regexPattern[0];
        if (firstChar == '\\')
        {
            // It's an escape sequence. Check what is being escaped.
            if (regexPattern.Length > 1)
            {
                char escapedChar = regexPattern[1];
                // In regex, \w (word) and \d (digit) represent word characters.
                // Other common escapes like \s, \n, \t, \. are not word characters.
                if (escapedChar == 'w' || escapedChar == 'd')
                {
                    prependBoundary = true;
                }
            }
        }
        // In C#, the \w shorthand is equivalent to [a-zA-Z0-9_].
        else if (char.IsLetterOrDigit(firstChar) || firstChar == '_')
        {
            // It's a literal character that is part of a word.
            prependBoundary = true;
        }

        // --- Check if the pattern ENDS with a word character ---

        char lastChar = regexPattern[regexPattern.Length - 1];

        // Check for single-character patterns that are word characters, like "a" or "_".
        if (regexPattern.Length == 1)
        {
            if (char.IsLetterOrDigit(lastChar) || lastChar == '_')
            {
                appendBoundary = true;
            }
        }
        else // Pattern has 2 or more characters
        {
            char penultimateChar = regexPattern[regexPattern.Length - 2];
            if (penultimateChar == '\\')
            {
                // The end of the string is an escape sequence, like \d or \n or \\.
                // Check if the sequence itself represents a word character.
                if (lastChar == 'w' || lastChar == 'd')
                {
                    appendBoundary = true;
                }
                // Note: If the pattern ends in \\, 'lastChar' is '\', which is not a 
                // letter/digit, so it's correctly ignored.
            }
            else if (char.IsLetterOrDigit(lastChar) || lastChar == '_')
            {
                // The last character is a literal that is part of a word.
                appendBoundary = true;
            }
        }

        // --- Build the final string ---

        // If no changes are needed, return the original string to avoid allocation.
        if (!prependBoundary && !appendBoundary)
        {
            return regexPattern;
        }

        var sb = new StringBuilder();
        if (prependBoundary)
        {
            sb.Append(@"\b");
        }

        sb.Append(regexPattern);

        if (appendBoundary)
        {
            sb.Append(@"\b");
        }

        return sb.ToString();
    }
}

