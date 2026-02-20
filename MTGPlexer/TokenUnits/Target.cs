namespace MTGPlexer.TokenUnits;

[Dependent]
public class Target : TokenUnit
{
    public override Snippet[] Snippets => [Prop(IsAny), "target", Prop(TargetableEntity)];

    [RegexPattern("any")]
    public bool IsAny { get; set; }

    [Optional]
    public TargetableEntity TargetableEntity { get; set; }
}
