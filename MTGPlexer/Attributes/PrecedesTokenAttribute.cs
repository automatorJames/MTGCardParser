namespace MTGPlexer.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class PrecedesTokenAttribute : TokenPlacementAttribute
{
    public override TokenPlacement Placement => TokenPlacement.PrecedesNext;
}

