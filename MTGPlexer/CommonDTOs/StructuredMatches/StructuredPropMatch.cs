namespace MTGPlexer.CommonDTOs.StructuredMatches;

public record StructuredPropMatch : StructuredMatchBase
{
    public RegexPropInfo RegexPropInfo { get; }
    public Match Match { get; }
    public override int AbsoluteStartInSource { get; }
    public override int AbsoluteEndInSource { get; }
    public override string SourceText { get; }
    public StructuredMatchBase[] Ancestors { get; } = [];
    public StructuredTokenRoot Root { get; }

    public StructuredPropMatch(StructuredMatchBase parent, Match match) : base(match)
    {
        if (parent is StructuredTokenRoot)
            Ancestors = [parent];
        else if (parent is StructuredPropMatch parentCapture)
            Ancestors = [.. parentCapture.Ancestors, parentCapture];

        Match = match;
        Root = (StructuredTokenRoot)Ancestors.First();
        AbsoluteStartInSource = parent.AbsoluteStartInSource + match.Index;
        AbsoluteEndInSource = AbsoluteStartInSource + match.Length;
        SourceText = Root.SourceText;
    }
}

