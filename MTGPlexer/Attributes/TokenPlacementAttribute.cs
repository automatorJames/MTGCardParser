namespace MTGPlexer.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public abstract class TokenPlacementAttribute : Attribute
{
    public abstract TokenPlacement Placement { get; }
}

public enum TokenPlacement
{
    Independent,
    FollowsPrevious,
    PrecedesNext,
    AlternatesFollowingAndPreceding
}