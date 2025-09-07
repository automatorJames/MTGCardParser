namespace MTGPlexer.TokenUnits;

[NoBoundary]
public class WhereVariableIs : TokenUnit
{
    public WhereVariableIs() : base(", where", nameof(VariableName), "is ") { }

    public VariableName VariableName { get; set; }
}