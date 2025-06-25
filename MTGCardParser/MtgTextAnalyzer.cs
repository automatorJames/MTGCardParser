/*namespace MTGCardParser;

/// <summary>
/// Implements a greedy algorithm to discover the most impactful text patterns
/// in a corpus of MTG card text.
/// </summary>
public class MtgTextAnalyzer
{
    public static List<string> GetCoveragePatterns(List<Card> cards)
    {
        var analyzer = new MtgTextAnalyzer();
        return analyzer.GenerateCoveragePatterns(cards);
    }

    /// <summary>
    /// Analyzes a list of cards to produce an ordered list of text patterns that
    /// provide the most coverage in a greedy fashion.
    /// </summary>
    /// <param name="cards">A list of all card objects to analyze.</param>
    /// <returns>An ordered list of patterns, from most to least impactful.</returns>
    public List<string> GenerateCoveragePatterns(List<Card> cards)
    {
        // --- PHASE 1: INITIALIZATION & DATA STRUCTURING ---

        // 1. Create the Universe of Clauses to be covered.
        // We use a HashSet for efficient removal.
        var uncoveredClauses = new HashSet<string>(cards.SelectMany(c => c.CleanedLines).Where(l => !string.IsNullOrWhiteSpace(l)).Distinct());

        // 2. Create the Pool of Candidate Patterns.
        // Note: Your Card.GetUnmatchedSegmentCombinations() is very broad. For performance on a large
        // dataset, you might want to constrain it, but the algorithm holds.
        var candidatePatterns = new HashSet<string>(cards.SelectMany(c => c.SegmentCombinations).Distinct());

        // 3. Build the powerful Reverse Index.
        Console.WriteLine("Building reverse index...");
        var patternToClausesIndex = new Dictionary<string, List<string>>();
        foreach (var pattern in candidatePatterns)
        {
            // This can be slow. For a huge number of patterns, this is the main bottleneck.
            // A more advanced implementation might use a Suffix Tree or Aho-Corasick data structure.
            var containingClauses = uncoveredClauses.Where(clause => clause.Contains(pattern)).ToList();
            if (containingClauses.Any())
            {
                patternToClausesIndex[pattern] = containingClauses;
            }
        }
        Console.WriteLine($"Index built. Found {patternToClausesIndex.Count} effective patterns.");

        var resultingPatterns = new List<string>();
        int iteration = 1;

        // --- PHASE 2: THE ITERATIVE GREEDY SELECTION LOOP ---
        while (uncoveredClauses.Any())
        {
            Console.WriteLine($"--- Iteration {iteration++}: Uncovered clauses remaining: {uncoveredClauses.Count} ---");

            PatternCandidate bestCandidate = FindBestCandidate(uncoveredClauses, patternToClausesIndex);

            if (bestCandidate == null || bestCandidate.Score == 0)
            {
                // This can happen if remaining clauses have no matching patterns in our pool
                // (e.g., they are single-word lines which we ignored).
                Console.WriteLine("No more valuable patterns found. Breaking.");
                // Optionally, add the remaining clauses as literal patterns
                resultingPatterns.AddRange(uncoveredClauses);
                break;
            }

            // 2. Process the Winner
            Console.WriteLine($"Selected pattern: '{bestCandidate.Pattern}' with score {bestCandidate.Score:F2} (Covers {bestCandidate.CoveredClauses.Count} clauses)");
            resultingPatterns.Add(bestCandidate.Pattern);

            // Mark all clauses covered by this pattern as "covered" by removing them.
            foreach (var clause in bestCandidate.CoveredClauses)
            {
                uncoveredClauses.Remove(clause);
            }
        }

        Console.WriteLine("Analysis complete.");
        return resultingPatterns;
    }

    private PatternCandidate FindBestCandidate(
        HashSet<string> uncoveredClauses,
        Dictionary<string, List<string>> patternToClausesIndex)
    {
        PatternCandidate bestCandidate = null;

        // We can parallelize this calculation as each pattern's score is independent.
        var candidates = patternToClausesIndex.Keys.AsParallel().Select(pattern =>
        {
            // Find which clauses are still in the uncovered set.
            var stillUncovered = patternToClausesIndex[pattern]
                .Where(clause => uncoveredClauses.Contains(clause))
                .ToList();

            if (!stillUncovered.Any())
            {
                return null;
            }

            // Score = (length of pattern in words) * (number of clauses it covers)
            // We add a small epsilon to the length to favor longer patterns in case of a tie.
            double score = (pattern.Split(' ').Length + 0.01) * stillUncovered.Count;

            return new PatternCandidate
            {
                Pattern = pattern,
                Score = score,
                CoveredClauses = stillUncovered
            };
        }).Where(c => c != null).ToList();

        if (candidates.Any())
        {
            // Find the candidate with the maximum score.
            bestCandidate = candidates.Aggregate((max, next) => next.Score > max.Score ? next : max);
        }

        return bestCandidate;
    }
}

*/