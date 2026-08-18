namespace MTGPlexer.TokenUnitPrimitives;

public abstract class OneOfBase : TokenUnit
{
    public override Joiner Joiner => Joiner.Pipe;
}