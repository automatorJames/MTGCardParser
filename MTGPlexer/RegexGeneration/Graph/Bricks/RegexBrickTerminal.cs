namespace MTGPlexer.RegexGeneration.Graph.Bricks;

public class RegexBrickTerminal : RegexBrick
{
    public object Value { get; }
    public int PositionAmongSiblings { get; set; }
    public int PositionAmongSynonymns { get; set; }

    public RegexBrickTerminal(
        RegexNode parentNode, 
        string regex, 
        string comment, 
        object value, 
        int positionAmongSiblings, 
        int positionAmongSynonyms)
        : base(parentNode, regex, comment)
    {
        Value = value;
        PositionAmongSiblings = positionAmongSiblings;
        PositionAmongSynonymns = positionAmongSynonyms;
    }
}
