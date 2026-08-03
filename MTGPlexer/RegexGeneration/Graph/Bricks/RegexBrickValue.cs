namespace MTGPlexer.RegexGeneration.Graph.Bricks;

public class RegexBrickValue : RegexBrick
{
    public object Value { get; }

    public RegexBrickValue(RegexNode parentNode, string regex, string comment, object value) 
        : base(parentNode, regex, comment)
    {
        Value = value;
    }
}