namespace MTGPlexer.GeneralDTOs;

public record StructuredSubCapture : StructuredMatch
{
    public Match ContainingMatch { get; }
    public Capture SubCapture { get; }

    public StructuredSubCapture(Type type, Match match, StructuredMatch parentMatch, Capture subCapture) : base(type, match, parentMatch)
    {
        ContainingMatch = parentMatch.Match;
        SubCapture = subCapture;
    }

    protected override void SetOriginalTextAndPosition()
    {
        var parent = Ancestors.Last();
        OriginalText = parent.OriginalText;
        Index = parent.Index + Match.Index;
        Length = parent.Length; // todo: fix all this positional logic
        End = Index + Length;
    }
}

