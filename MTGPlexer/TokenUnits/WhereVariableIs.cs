namespace MTGPlexer.TokenUnits;

[RegexBoundaryOptionAtrribute(BoundaryOption.Omit)]
public class WhereVariableIs : TokenUnit
{
    public WhereVariableIs() : base(", where", nameof(VariableName), "is ") { }

    public VariableName VariableName { get; set; }
}