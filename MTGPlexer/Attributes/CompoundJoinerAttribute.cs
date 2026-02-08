namespace MTGPlexer.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property)]
public class CompoundJoinerAttribute : Attribute
{
    public Joiner Joiner { get; }

    public CompoundJoinerAttribute(Joiner joiner)
    {
        Joiner = joiner;
    }
}