namespace MTGPlexer.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class FollowsTokenAttribute : TokenPlacementAttribute
{
    public override TokenPlacement Placement => TokenPlacement.FollowsPrevious;
}

