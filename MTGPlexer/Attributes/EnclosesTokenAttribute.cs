namespace MTGPlexer.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class EnclosesTokenAttribute : TokenPlacementAttribute
{
    public override TokenPlacement Placement => TokenPlacement.AlternatesFollowingAndPreceding;
}

