namespace MTGCardParser.TokenCaptures;

public class IfYouDo : TokenCaptureBase<IfYouDo>
{
    public override RegexTemplate<IfYouDo> RegexTemplate => new("if you do,");
}