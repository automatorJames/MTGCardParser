namespace MTGCardParser.TokenCaptures;

public class This : TokenCaptureBase<This>
{
    public override RegexTemplate<This> RegexTemplate => new(@"\{this\}");
}