namespace MTGPlexer;

public class CardTokenizer : Tokenizer
{
    public CardTokenizer(List<Type> orderedTypes) : base(orderedTypes) { }

    public TokenizedCard TokenizeCard(Card card)
    {
        var processedClauses = new List<TokenizedClause>();

        for (int i = 0; i < card.FormattedLinesLower.Length; i++)
        {
            var lineTextLower = card.FormattedLinesLower[i];
            var lineTextOriginal = card.FormattedLines[i];

            if (string.IsNullOrWhiteSpace(lineTextLower))
                continue;

            var rootTokens = Tokenize(lineTextLower);
            rootTokens.ForEach(x => x.PrependCardPathAllLevels(card.Name.Replace(' ', '_'), i));
            processedClauses.Add(new(rootTokens, i, lineTextOriginal));
        }

        return new(card, processedClauses);
    }

    public List<TokenCaptureSummary> GetCardTokenAnalysis(Card card)
    {
        List<TokenCaptureSummary> list = [];
        TokenizedCard tokenizeCard = TokenizeCard(card);

        foreach (var clause in tokenizeCard.Clauses)
            foreach (var token in clause.Tokens)
                list.Add(TokenCaptureSummary.CreateFrom(token, clause.OriginalText));

        return list;
    }

}