namespace Glyphotype.Tokenizers;

public class Tokenizer
{
    private readonly List<Type> _orderedTopLevelTypes;
    private readonly List<Type> _dependentTypes;
    private static readonly Dictionary<int, Regex> _unmatchedRegexCache = [];

    public Tokenizer(List<Type> orderedTopLevelTypes, List<Type> dependentTypes)
    {
        _orderedTopLevelTypes = orderedTopLevelTypes;
        _dependentTypes = dependentTypes;
    }

    public List<CaptureUnit> Tokenize(
        string sourceText, 
        int? scopeStart = null, int? 
        scopeEnd = null, 
        Type scopeToType = null,
        bool includeDependentTypes = false)
    {
        if (string.IsNullOrEmpty(sourceText))
            throw new Exception("Source text may not be null or empty");

        var tokens = new List<CaptureUnit>();
        int currentIndex = scopeStart ?? 0;
        int endIndex = scopeEnd ?? sourceText.Length;
        int unmatchedStartIndex = -1;

        // Fixed anchor for the scope this call is tokenizing, used below to gate MustMatchWholeLine
        // types: currentIndex advances as tokens get committed or unmatched text gets skipped, but a
        // MustMatchWholeLine type is only a valid candidate on the very first attempt at this scope's own
        // start - once anything (a token or a ratchet skip) has consumed part of the scope, no match
        // starting after that point could still be "the whole line" by itself.
        int scopeStartIndex = currentIndex;

        var candidateTypes = _orderedTopLevelTypes.ToList();

        if (includeDependentTypes)
            candidateTypes.AddRange(_dependentTypes);

        // Pre-filter types to avoid repeating logic inside the while loop
        var filteredTypes = 
            (scopeToType != null && scopeToType != typeof(Glyph)) ? candidateTypes.Where(x => x.IsAssignableTo(scopeToType)).ToList()
            : candidateTypes;

        while (currentIndex < endIndex)
        {
            bool matched = false;

            foreach (var type in filteredTypes)
            {
                var rootNode = GlyphTypeRegistry.RegexGraphIncludingDependents[type];

                if (rootNode.MustMatchWholeLine && currentIndex != scopeStartIndex)
                    continue;

                if (rootNode.TryMatch(sourceText, currentIndex, endIndex, out var token))
                {
                    // --- COMMIT PHASE ---
                    FlushUnmatched(sourceText, tokens, ref unmatchedStartIndex, currentIndex);

                    tokens.Add(token);

                    // Advance by the length of the matched token
                    // Assuming Glyph contains the length or the raw text
                    currentIndex += token.CaptureValue.Length;

                    unmatchedStartIndex = -1;
                    matched = true;
                    break;
                }
            }

            if (!matched)
            {
                if (unmatchedStartIndex == -1)
                    unmatchedStartIndex = currentIndex;

                // Ratchet logic: skip to the next space
                int nextSpaceIndex = sourceText.IndexOf(' ', currentIndex);

                if (nextSpaceIndex == -1 || nextSpaceIndex >= endIndex)
                    currentIndex = endIndex;
                else
                    currentIndex = nextSpaceIndex + 1;
            }
        }

        FlushUnmatched(sourceText, tokens, ref unmatchedStartIndex, endIndex);

        return tokens;
    }

    private void FlushUnmatched(string sourceText, List<CaptureUnit> tokens, ref int unmatchedStartIndex, int flushUntilIndex)
    {
        if (unmatchedStartIndex == -1 || unmatchedStartIndex >= flushUntilIndex)
        {
            unmatchedStartIndex = -1;
            return;
        }

        int length = flushUntilIndex - unmatchedStartIndex;

        if (length <= 0)
        {
            unmatchedStartIndex = -1;
            return;
        }

        UnmatchedString defualtUnmatchedString = new(sourceText, unmatchedStartIndex, length);
        tokens.Add(defualtUnmatchedString);

        unmatchedStartIndex = -1;
    }
}