/*using MTGPlexer.Data;

public class CardTokenizer : Tokenizer
{
    public CardTokenizer(List<Type> orderedTypes) : base(orderedTypes) { }

    public ProcessedCard TokenizeCard(Card card)
    {
        var processedLines = new List<ProcessedLine>();
        for (int i = 0; i < card.FormattedLinesLower.Length; i++)
        {
            var lineText = card.FormattedLinesLower[i];

            if (string.IsNullOrWhiteSpace(lineText))
                continue;

            var lineTokens = TokenTypeRegistry.Tokenize(lineText, originalTextOnly);

            _hydratedTokenUnits.AddRange(lineTokens);

            // This single method call performs both analyses for the line.
            (var spanRoots, var spanOccurrences) = AnalyzeLine(card, lineTokens, i);

            processedLines.Add(new ProcessedLine
            {
                Card = card,
                LineIndex = i,
                EvaluatedText = lineText,
                SourceTokens = lineTokens,
                SpanRoots = spanRoots,
                SpanOccurrences = spanOccurrences
            });


        }
    }
}*/