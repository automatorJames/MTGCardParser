namespace MTGCardParser;

internal class Program
{
    static void Main(string[] args)
    {
        string breakOnWordToken = "target";

        var cards = DataGetter.GetCards(1, true);
        TryTokenize(cards, breakOnWordToken);
    }

    static void TryTokenize(List<Card> cards, string breakOnWordToken = null)
    {
        var tokenizer = MtgTokenizer.Create();

        // pretty-print for AST debug
        var jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            Converters = { new JsonStringEnumConverter() }
        };

        // accumulator for all unmatched Text spans
        var unmatched = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

        // --- Execution Loop ---
        foreach (var card in cards)
        {
            Console.WriteLine($"--- Tokenizing: {card.Name} ---");

            // Console.Clear(); <-- this works to clear

            var tokens = tokenizer.Tokenize(card.Text);

            // 2. Collect runs of MtgToken.Text
            var buffer = new List<string>();
            foreach (var token in tokens)
            {
                if (token.Kind == MtgToken.Text)
                {
                    if (!string.IsNullOrEmpty(breakOnWordToken) && token.ToStringValue() == breakOnWordToken)
                    {
                        Console.WriteLine(card.Text);
                        Debugger.Break();
                    }

                    buffer.Add(token.ToStringValue());
                }
                else
                {
                    if (buffer.Count > 0)
                    {
                        var span = string.Join(" ", buffer);
                        unmatched[span] = unmatched.GetValueOrDefault(span) + 1;
                        buffer.Clear();
                    }
                }
            }
            if (buffer.Count > 0)
            {
                var span = string.Join(" ", buffer);
                unmatched[span] = unmatched.GetValueOrDefault(span) + 1;
                buffer.Clear();
            }
        }

        // --- Reporting unmatched spans ---
        Console.Clear(); // <-- this doesn't work to clear
        Console.WriteLine("=== Unmatched Text-token spans ===");

        var unmatchedKvs = unmatched
                             .OrderByDescending(kv => kv.Value)
                             .ThenBy(kv => kv.Key, StringComparer.OrdinalIgnoreCase);

        foreach (var kv in unmatchedKvs)
            Console.WriteLine($"{kv.Value}: {kv.Key}");
    }

    public static void Parse()
    {

        //// --- Execution Loop ---
        //foreach (var card in cards)
        //{
        //    Console.WriteLine($"--- Parsing: {card.Name} ('{card.Text}') ---");
        //
        //    try
        //    {
        //        // 1. Lexer: Turn the raw string into a list of tokens
        //        var tokens = tokenizer.Tokenize(card.Text);
        //
        //        // 2. Parser: Consume the token list to produce an AST
        //        var ability = MtgParser.Ability.Parse(tokens);
        //
        //        // 3. Output: Display the result as a formatted JSON object
        //        var json = JsonSerializer.Serialize(ability, jsonOptions);
        //
        //        Console.WriteLine("Success!");
        //        Console.WriteLine(json);
        //    }
        //    catch (ParseException ex)
        //    {
        //        // Superpower provides helpful, human-readable error messages
        //        Console.WriteLine($"Failed to parse: {ex.Message}");
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"An unexpected error occurred: {ex.Message}");
        //    }
        //    Console.WriteLine(new string('-', 50));
        //    Console.WriteLine();
        //}
    }
}
