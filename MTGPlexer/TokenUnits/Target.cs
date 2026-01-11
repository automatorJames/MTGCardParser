namespace MTGPlexer.TokenUnits;

[Dependent]
public class Target : TokenUnit
{
    protected override Snippet[] Snippets => [Prop(IsAny), "target", Prop(TargetableEntity)];

    [RegexPattern("any")]
    public bool IsAny { get; set; }

    [OptionalComponent]
    public TargetableEntity TargetableEntity { get; set; }
}
