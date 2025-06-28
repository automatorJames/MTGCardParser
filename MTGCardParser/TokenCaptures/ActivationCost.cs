namespace MTGCardParser.TokenCaptures;

public class ActivationCost : TokenCaptureBase<ActivationCost>
{
    public override RegexTemplate<ActivationCost> RegexTemplate => new("^", nameof(ActivationCostSegment), ":");

    [RegexPattern("[^:]+")]
    public TokenSegment ActivationCostSegment { get; set; }
}

