using System;
using System.Collections.Generic;
using System.Linq;

public record TemplateCandidate(
    string Prefix,
    List<string> NextWords,
    int Support,
    int Branchiness,
    int PrefixWordCount,
    int Fitness
);

public static class TemplateSuggester
{
    /// <summary>
    /// From your corpus of unmatched spans (with global counts),
    /// returns all prefix‑template candidates sorted by descending fitness.
    ///
    /// Fitness = Support × Branchiness × PrefixWordCount
    ///   • Support = how many spans share this prefix
    ///   • Branchiness = how many distinct next‑words follow it
    ///   • PrefixWordCount = number of words in the prefix
    ///
    /// You get everything; no thresholds.  Low‑fitness items simply sort to the bottom.
    /// </summary>
    public static List<TemplateCandidate> SuggestTemplates(
        List<UnmatchedSpanCount> spans)
    {
        // 1) Build the trie of all prefixes
        var trie = new PrefixTree();
        foreach (var span in spans)
            trie.Add(span.Text, span.OccurrenceCount);

        // 2) Extract every prefix node (except root)
        var raw = trie.GetAllCandidates();

        // 3) Compute fitness and sort descending
        var scored = raw
            .Select(x =>
            {
                int fitness = x.Support * x.Branchiness * x.PrefixWordCount;
                return new TemplateCandidate(
                    Prefix: x.Prefix,
                    NextWords: x.NextWords,
                    Support: x.Support,
                    Branchiness: x.Branchiness,
                    PrefixWordCount: x.PrefixWordCount,
                    Fitness: fitness
                );
            })
            .OrderByDescending(c => c.Fitness)
            .ToList();

        return scored;
    }

    // ────────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// A simple word‑trie that tracks:
    ///  • Count = how many spans pass through each node
    ///  • Children = next‑word branches
    /// </summary>
    private class PrefixTree
    {
        private class Node
        {
            public int Count;
            public Dictionary<string, Node> Children
                = new(StringComparer.Ordinal);
        }

        private readonly Node _root = new();

        /// <summary>
        /// Inserts one span (with its global frequency) into the trie,
        /// bumping Count on every prefix node.
        /// </summary>
        public void Add(string span, int freq)
        {
            var words = span
                .Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var node = _root;
            node.Count += freq;

            foreach (var w in words)
            {
                if (!node.Children.TryGetValue(w, out var child))
                {
                    child = new Node();
                    node.Children[w] = child;
                }
                child.Count += freq;
                node = child;
            }
        }

        /// <summary>
        /// Walks the trie and returns a tuple for every non‑root node:
        ///  • Prefix = the words from root to this node as a string  
        ///  • NextWords = list of immediate children keys  
        ///  • Support = node.Count  
        ///  • Branchiness = node.Children.Count  
        ///  • PrefixWordCount = number of words in the prefix  
        /// </summary>
        public IEnumerable<(string Prefix, List<string> NextWords, int Support, int Branchiness, int PrefixWordCount)>
            GetAllCandidates()
        {
            return Recurse(_root, new List<string>());
        }

        private IEnumerable<(string, List<string>, int, int, int)> Recurse(
            Node node,
            List<string> path)
        {
            var results = new List<(string, List<string>, int, int, int)>();

            // skip the root (empty path), but record every other node
            if (path.Count > 0)
            {
                results.Add((
                    Prefix: string.Join(" ", path),
                    NextWords: node.Children.Keys.ToList(),
                    Support: node.Count,
                    Branchiness: node.Children.Count,
                    PrefixWordCount: path.Count
                ));
            }

            // descend
            foreach (var kv in node.Children)
            {
                path.Add(kv.Key);
                results.AddRange(Recurse(kv.Value, path));
                path.RemoveAt(path.Count - 1);
            }

            return results;
        }
    }
}
