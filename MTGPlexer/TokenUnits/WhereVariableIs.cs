namespace MTGPlexer.TokenUnits;

[RegexBoundaryOptionAtrribute(BoundaryOption.None)]
public class WhereVariableIs : TokenUnit
{
    public override Snippet[] Snippets => [", where", Prop(VariableName), "is "];

    public VariableName VariableName { get; set; }
}