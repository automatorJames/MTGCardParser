namespace MTGPlexer.TokenUnits;

[RegexBoundaryOptionAtrribute(BoundaryOption.Omit)]
public class WhereVariableIs : TokenUnit
{
    protected override string[] Snippets => [", where", nameof(VariableName), "is "];

    public VariableName VariableName { get; set; }
}