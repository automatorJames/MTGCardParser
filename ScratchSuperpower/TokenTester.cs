using Superpower.Model;

namespace MTGCardParser;

public class TokenTester
{
    private readonly List<Card> _cards;
    private readonly int? _showTopXLines;
    private readonly int _contextWidth = 2;

    private readonly Dictionary<string, (int Count, string FirstCard, string Snippet)> _unmatched
        = new(StringComparer.OrdinalIgnoreCase);

    private int _totalTextTokens;
    private int _totalNonTextTokens;

    public TokenTester(int? showTopXLines = null, int? maxSetSequence = null, bool ignoreEmptyText = false)
    {
        _cards = DataGetter.GetCards(maxSetSequence, ignoreEmptyText);
        _showTopXLines = showTopXLines;
    }

    public void Process()
    {
        foreach (var card in _cards)
        {
            var tokens = TokenCaptureFactory.CleanAndTokenize(card.Text).ToList();
            TrackTokenCounts(tokens);
            ExtractUnmatchedRuns(card, tokens);
        }

        PrintCoverage();
        PrintUnmatchedTable();
    }

    private void TrackTokenCounts(List<Token<MtgToken>> tokens)
    {
        foreach (var token in tokens)
        {
            if (token.Kind == MtgToken.Text)
                _totalTextTokens++;
            else
                _totalNonTextTokens++;
        }
    }

    private void ExtractUnmatchedRuns(Card card, List<Token<MtgToken>> tokens)
    {
        var buffer = new List<string>();
        foreach (var token in tokens)
        {
            var text = token.ToStringValue();
            if (token.Kind == MtgToken.Text)
                buffer.Add(text);
            else
            {
                FlushBuffer(card, buffer);
                buffer.Clear();
            }
        }
        FlushBuffer(card, buffer);
    }

    private void FlushBuffer(Card card, List<string> buffer)
    {
        if (buffer.Count == 0) return;
        var span = string.Join(" ", buffer);
        var snippet = BuildSnippet(card.Text, span, _contextWidth);

        if (_unmatched.TryGetValue(span, out var info))
            _unmatched[span] = (info.Count + 1, info.FirstCard, info.Snippet);
        else
            _unmatched[span] = (1, card.Name, snippet);
    }

    private static string BuildSnippet(string text, string span, int width)
    {
        text = text.Replace("\n", " (newline) ");
        int idx = text.IndexOf(span, StringComparison.OrdinalIgnoreCase);
        if (idx < 0) return span;

        // left boundary
        int start = 0, pos = idx;
        for (int i = 0; i < width && pos > 0; i++)
        {
            int prev = text.LastIndexOf(' ', pos - 1);
            if (prev < 0) break;
            start = prev + 1;
            pos = prev;
        }

        // right boundary
        int end = text.Length;
        pos = idx + span.Length;
        for (int i = 0; i < width && pos < text.Length; i++)
        {
            int next = text.IndexOf(' ', pos);
            if (next < 0) break;
            end = next;
            pos = next;
        }

        return text[start..end].Trim();
    }

    private void PrintCoverage()
    {
        int totalTokens = _totalTextTokens + _totalNonTextTokens;
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"Matched:                    {_totalNonTextTokens}");
        Console.WriteLine($"Unmatched:                  {_totalTextTokens}");
        Console.WriteLine($"Coverage:                   {(_totalNonTextTokens * 100.0 / totalTokens):F1}%");
        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.White;
    }

    private void PrintUnmatchedTable()
    {
        var results = _unmatched
            .OrderByDescending(kv => kv.Value.Count)
            .ThenBy(kv => kv.Key, StringComparer.OrdinalIgnoreCase)
            .ToList();

        if (_showTopXLines.HasValue)
            results = results.Take(_showTopXLines.Value).ToList();

        Console.WriteLine($"{"Cnt",5}  {"Token",-25}  {"First Card",-30}  Snippet");
        Console.WriteLine(new string('-', 90));

        foreach (var kv in results)
        {
            var span = kv.Key;
            var (count, firstCard, snippet) = kv.Value;
            var colored = $"\u001b[94m{span}\u001b[0m";
            var display = snippet.Replace(span, colored);

            Console.WriteLine(
                $"{count,5}  {span,-25}  {firstCard,-30}  {display}"
            );
        }
    }
}


