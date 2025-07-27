using System.Text;

/// <summary>
/// SpanOccurrenceCounter analyzes a collection of card texts to extract meaningful repeated phrases.
///
/// Each input card contains one or more "unmatched spans"—strings of space-separated words.
/// The goal is to identify all contiguous word sequences (i.e. sub-spans) that are worth displaying:
///
/// • If a sub-span appears in more than one card, it's only worth keeping if:
///   – it occurs more than once *across the entire corpus*, AND
///   – it cannot be lengthened by one or more words *without reducing its global count*
///   ⇒ These are called "maximal repeated sub-spans"
///
/// • Additionally, all original full card spans (whether repeated or not) are included for completeness.
///
/// To do this efficiently (even with large numbers of cards), the implementation builds a suffix automaton:
/// a compact state machine that remembers every unique substring of the input, along with how often it occurs.
///
/// HOW IT WORKS (in plain terms):
/// 1. Converts every card’s words into numbers, so we can process them faster
/// 2. Stitches all cards together into one long stream, with separators to prevent cross-card matches
/// 3. Builds a memory-efficient machine that tracks every possible contiguous word sequence
/// 4. Counts how often each sequence occurs across all cards
/// 5. Filters the results:
///    – Keeps only "maximal repeated spans": phrases that are repeated and can’t be extended without dropping the count
/// 6. Adds in all full-card texts (even if they’re not repeated) to make sure no input is lost
///
/// The final result is a dictionary mapping strings (the span) to the number of times they occurred,
/// with the guarantee that no span is redundant (i.e., entirely contained by a longer span with the same count).
///
/// This approach is significantly faster and more memory-efficient than scanning every possible substring manually,
/// especially as the number of cards grows large.
/// </summary>
public class SpanOccurrenceCounter
{
    // Holds the automaton state
    private class State
    {
        public int[] Next;      // transitions on word‑ID
        public int Link;        // suffix link
        public int Len;         // max length of substrings in this class
        public long Count;      // end‑pos count
        public int FirstPos;    // one end position of a substring in this class

        public State(int alphabetSize)
        {
            Next = new int[alphabetSize];
            for (int i = 0; i < alphabetSize; i++) Next[i] = -1;
            Link = -1;
            Len = 0;
            Count = 0;
            FirstPos = -1;
        }
    }

    public static List<UnmatchedSpanCount> GetUnmatchedSpanCounts(List<CardDigest> digestedCards)
    {
        var dict = CountSpans(digestedCards);
        return dict.Select(x => new UnmatchedSpanCount(x.Key, x.Value)).ToList();
    }

    /// <summary>
    /// Given an IEnumerable of “whole spans” (your card texts),
    /// returns a dictionary mapping
    ///   span → global occurrence count
    /// containing exactly:
    ///  • every whole‑span (count = 1 for unique cards),
    ///  • every _maximal repeated_ sub‑span (count > 1).
    /// </summary>
    static Dictionary<string, int> CountSpans(List<CardDigest> digestedCards)
    {
        var lengthOrderedUnmatchedSpans = digestedCards
            .SelectMany(x => x.UnmatchedSpans)
            .OrderByDescending(x => x.Length)
        .ToList();

        // 1) Build a word→ID map (ID=1..), reserve 0 for separator
        var wordToId = new Dictionary<string, int>(StringComparer.Ordinal);
        var idToWord = new List<string> { null }; // index=0 unused
        int nextId = 1;

        // Tokenize cards into sequences of IDs
        var sequences = new List<int[]>();
        foreach (var text in lengthOrderedUnmatchedSpans)
        {
            var words = text.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var seq = new int[words.Length];
            for (int i = 0; i < words.Length; i++)
            {
                if (!wordToId.TryGetValue(words[i], out var id))
                {
                    id = nextId++;
                    wordToId[words[i]] = id;
                    idToWord.Add(words[i]);
                }
                seq[i] = id;
            }
            sequences.Add(seq);
        }

        // 2) Concatenate with sentinel ‘0’ between cards
        var concat = new List<int>();
        foreach (var seq in sequences)
        {
            concat.AddRange(seq);
            concat.Add(0);
        }

        // 3) Build suffix automaton
        int alpha = nextId;               // alphabet = [0..nextId-1]
        var states = new List<State>();
        states.Add(new State(alpha));     // state 0 = initial
        int last = 0;                     // the “active” state

        void Extend(int c)
        {
            int cur = states.Count;
            states.Add(new State(alpha));
            states[cur].Len = states[last].Len + 1;
            states[cur].Count = 1;
            states[cur].FirstPos = states[cur].Len - 1;

            int p = last;
            while (p >= 0 && states[p].Next[c] == -1)
            {
                states[p].Next[c] = cur;
                p = states[p].Link;
            }
            if (p == -1)
            {
                states[cur].Link = 0;
            }
            else
            {
                int q = states[p].Next[c];
                if (states[p].Len + 1 == states[q].Len)
                {
                    states[cur].Link = q;
                }
                else
                {
                    // clone state q
                    int clone = states.Count;
                    states.Add(new State(alpha));
                    states[clone].Len = states[p].Len + 1;
                    states[clone].Next = (int[])states[q].Next.Clone();
                    states[clone].Link = states[q].Link;
                    states[clone].Count = 0;  // clones start with 0; real ends go to cur
                    states[clone].FirstPos = states[q].FirstPos;

                    while (p >= 0 && states[p].Next[c] == q)
                    {
                        states[p].Next[c] = clone;
                        p = states[p].Link;
                    }
                    states[q].Link = states[cur].Link = clone;
                }
            }
            last = cur;
        }

        // feed the concatenated word‑IDs
        foreach (var c in concat) Extend(c);

        // 4) Compute end‑pos counts by “propagating” in descending‑length order
        var order = Enumerable.Range(0, states.Count)
                              .OrderByDescending(i => states[i].Len)
                              .ToArray();
        foreach (var i in order)
        {
            if (states[i].Link >= 0)
                states[states[i].Link].Count += states[i].Count;
        }

        // 5) Extract _maximal_ repeated spans (Count>1, no right‑extension at same count)
        var result = new Dictionary<string, int>(StringComparer.Ordinal);
        for (int i = 1; i < states.Count; i++)
        {
            if (states[i].Count <= 1)
                continue;

            bool hasSameCountExtension = false;
            for (int c = 0; c < alpha; c++)
            {
                int nxt = states[i].Next[c];
                if (nxt != -1 && states[nxt].Count == states[i].Count)
                {
                    hasSameCountExtension = true;
                    break;
                }
            }
            if (hasSameCountExtension)
                continue;

            // retrieve one representative substring of length = states[i].Len
            int endPos = states[i].FirstPos;
            int len = states[i].Len;
            int start = endPos - len + 1;

            // skip if it crosses a sentinel (0)
            bool skip = false;
            for (int k = start; k <= endPos; k++)
                if (concat[k] == 0) { skip = true; break; }
            if (skip)
                continue;

            // build the string
            var sb = new StringBuilder();
            for (int k = start; k <= endPos; k++)
            {
                if (k > start) sb.Append(' ');
                sb.Append(idToWord[concat[k]]);
            }
            result[sb.ToString()] = (int)states[i].Count;
        }

        // 6) Finally, add every whole‑span from input (they occur once unless duplicates exist)
        var wholeCounts = lengthOrderedUnmatchedSpans
            .GroupBy(s => s)
            .ToDictionary(g => g.Key, g => g.Count(), StringComparer.Ordinal);

        foreach (var kv in wholeCounts)
        {
            // if already present (because it was a repeated maximal substring),
            // we leave it; otherwise we add it with its whole‑span count.
            if (!result.ContainsKey(kv.Key))
                result[kv.Key] = kv.Value;
        }

        return result;
    }
}
