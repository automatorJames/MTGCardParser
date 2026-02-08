namespace MTGPlexer.RegexGeneration.Graph.Nodes;

public class TokenUnitCompoundNode : TokenUnitNode
{
    Joiner _joiner;

    public TokenUnitCompoundNode(RegexNode parentNode, TypeNavigation navigation) : base(parentNode, navigation)
    {
        _joiner = navigation.UnderlyingType.GetCustomAttribute<CompoundJoinerAttribute>()?.Joiner 
            ?? Joiner.Space;
    }

    public override void AppendRegexBricks(RegexCollector collector)
    {
        collector.Append(GroupOpenBrick);
        collector.AppendJoined(Children, GetJoinerBrick(_joiner));
        collector.Append(GroupCloseBrick);
    }

    public override string ToString() => base.ToString();
}