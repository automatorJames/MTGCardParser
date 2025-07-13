namespace MTGPlexer.TokenUnits;

[IgnoreInAnalysis]
public class DefaultUnmatchedString : TokenUnit
{
    public DefaultUnmatchedString() : base($"[^{string.Join("", RegexTemplate.Punctuation)}]+") { }
    //public DefaultUnmatchedString() : base(@"[^.,;""\s]+") { }
}

