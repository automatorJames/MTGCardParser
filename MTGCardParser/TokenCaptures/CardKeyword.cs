namespace MTGCardParser.TokenCaptures;

public class CardKeyword : TokenCaptureBase<CardKeyword>
{
    public override RegexTemplate<CardKeyword> RegexTemplate => new(nameof(Keyword));

    public Keyword? Keyword { get; set; }
}