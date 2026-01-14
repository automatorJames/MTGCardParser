namespace MTGPlexer.TokenUnits;

[RegexBoundaryOptionAtrribute(BoundaryOption.None)]
public class WhereVariableIs : TokenUnit
{
    protected override Snippet[] Snippets => [", where", Prop(VariableName), "is "];

    public VariableName VariableName { get; set; }
}