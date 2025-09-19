namespace MTGPlexer.Tokenizers;

public class CardTokenizer : Tokenizer
{
    public CardTokenizer(List<Type> orderedTypes) : base(orderedTypes) { }

    public TokenizedCard TokenizeCard(Card card)
    {
        var processedClauses = new List<TokenizedClause>();
        var cardNamePath = card.Name.Replace(' ', '_');

        for (int i = 0; i < card.FormattedLinesLower.Length; i++)
        {
            var lineTextLower = card.FormattedLinesLower[i];
            var lineTextOriginal = card.FormattedLines[i];

            if (string.IsNullOrWhiteSpace(lineTextLower))
                continue;

            // Tokenize the line. The resulting tokens have local paths.
            var rootTokens = Tokenize(lineTextLower);

            var pathPrefix = $"{cardNamePath}[{i}]";
            processedClauses.Add(new TokenizedClause(rootTokens, i, lineTextOriginal, pathPrefix));
        }

        return new TokenizedCard(card, processedClauses);
    }

    // This method still works, but the paths in the resulting DTOs will now be correctly prepended
    // because it calls the updated TokenCaptureSummary.CreateFrom method internally.
    public List<SpanRoot> GetCardTokenAnalysis(Card card)
    {
        var list = new List<SpanRoot>();
        var tokenizedCard = TokenizeCard(card);

        foreach (var clause in tokenizedCard.Clauses)
        {
            foreach (var token in clause.Tokens)
            {
                // Pass the new context parameters to the factory.
                list.Add(TokenCaptureSummary.CreateFrom(token, clause.OriginalText, card.Name, clause.ClauseIndex));
            }
        }

        return list;
    }
}